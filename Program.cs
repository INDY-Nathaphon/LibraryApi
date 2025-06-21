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
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;
using LibraryApi.BusinessLogic.Service.TokenBlacklist;
using LibraryApi.Domain;
using LibraryApi.Domain.CurrentUserProvider;
using LibraryApi.Domain.Entities;
using LibraryApi.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP
    options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // HTTPS
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureAppSettings(builder.Configuration);

#region Authorization

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var Secret = jwtSettings["Secret"] ?? string.Empty;
var issuer = jwtSettings["Issuer"];

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        //options.Authority = "https://your-auth-server.com"; // สำหรับกรณีใช้ Identity Server
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
    });

builder.Services.AddAuthorization();

#endregion

#region Database

var connectionString = builder.Configuration.GetConnectionString("DbConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "AuthService_";
});

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

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

#region Middleware

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CurrentUserMiddleware>();

#endregion

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
