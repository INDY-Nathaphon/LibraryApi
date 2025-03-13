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
using LibraryApi.BusinessLogic.Middleware;
using LibraryApi.BusinessLogic.Service.TokenBlacklist;
using LibraryApi.BusinessLogic.TransactionManager;
using LibraryApi.Domain;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

#region Database

var connectionString = builder.Configuration.GetConnectionString("DbConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

#endregion

#region Add Service

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();  

builder.Services.AddScoped<IUserFacade, UserFacade>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAuthenticationFacade, AuthenticationFacade>();
builder.Services.AddScoped<IAuthenticationService, Authenticationservice>();

builder.Services.AddScoped<IBookFacade,BookFacade >();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<ILibraryFacade, LibraryFacade>();
builder.Services.AddScoped<ILibraryService, LibraryService>();

builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
