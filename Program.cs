using LibraryApi.BusinessLogic.Implement.Authentication.Facade;
using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Implement.Authentication.Service;
using LibraryApi.BusinessLogic.Implement.Book.Facade;
using LibraryApi.BusinessLogic.Implement.Book.Interface;
using LibraryApi.BusinessLogic.Implement.Book.Service;
using LibraryApi.BusinessLogic.Implement.Library.Facade;
using LibraryApi.BusinessLogic.Implement.Library.Interface;
using LibraryApi.BusinessLogic.Implement.Library.Service;
using LibraryApi.BusinessLogic.Implement.User.Facade;
using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.BusinessLogic.Implement.User.Service;
using LibraryApi.BusinessLogic.Infrastructure.Redis;
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;
using LibraryApi.Common.Helpers.Attribute;
using LibraryApi.Domain;
using LibraryApi.Domain.CurrentUserProvider;
using LibraryApi.Domain.Entities;
using LibraryApi.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP
    options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // HTTPS
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureAppSettings(builder.Configuration);

#region AppSettings

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<AppSettings>>().Value
);

#endregion

#region Authorization

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var Secret = jwtSettings["Secret"] ?? string.Empty;
var issuer = jwtSettings["Issuer"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";  // หรือ JwtBearerDefaults.AuthenticationScheme
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
        RequireExpirationTime = true,
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
    options.CallbackPath = "/signin-google";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("LibraryAccessPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new LibraryAccessRequirement());
    });
});

builder.Services.AddScoped<IAuthorizationHandler, LibraryAccessHandler>();

#endregion

#region Database

var connectionString = builder.Configuration.GetConnectionString("DbConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var reids = builder.Configuration.GetSection("Redis");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = reids["RedisConnectionString"] ?? string.Empty;
    return ConnectionMultiplexer.Connect(configuration);
});

#endregion

#region handlers

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("LibraryAccessPolicy", policy =>
    {
        policy.Requirements.Add(new LibraryAccessRequirement());
    });
});

builder.Services.AddScoped<IAuthorizationHandler, LibraryAccessHandler>();

#endregion

#region Add Service

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddScoped<ITransactionManagerService, TransactionManagerService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<DbContext, AppDbContext>();

builder.Services.AddScoped<IUserFacade, UserFacade>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAuthenticationFacade, AuthenticationFacade>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IBookFacade, BookFacade>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<ILibraryFacade, LibraryFacade>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

builder.Services.AddScoped<IRedisService, RedisService>();

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .WithOrigins() // เปลี่ยนตาม frontend ของคุณ
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); // สำหรับ cookie หรือ OAuth redirect
});

var app = builder.Build();

app.UseRouting();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Middleware

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CurrentUserMiddleware>();

#endregion

app.UseHttpsRedirection();

app.UseAuthentication();   // ตรวจสอบตัวตนก่อน
app.UseAuthorization();    // ตรวจสอบสิทธิ์หลังจากรู้ว่าเป็นใคร

app.MapControllers();

app.Run();
