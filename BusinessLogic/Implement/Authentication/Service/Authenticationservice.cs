using System.Data.Entity;
using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.Controllers.DTO.AUthenticationDTO;
using LibraryApi.Controllers.DTO.BaseDTO;
using LibraryApi.Domain;
using Microsoft.AspNetCore.Identity;
using static LibraryApi.BusinessLogic.Enum.Enums;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Service
{
    public class Authenticationservice : BaseService<Domain.Entities.BaseEntity>, IAuthenticationService
    {

        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Domain.Entities.User> _passwordHasher;
        public Authenticationservice(AppDbContext context, IPasswordHasher<Domain.Entities.User> passwordHasher) : base(context)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseDto> RegisterUserAsync(RegisterDto model)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                return new ResponseDto(false, "Email already exists");
            }

            if (model.AuthProvider != AuthProvider.Local && string.IsNullOrEmpty(model.OAuthId))
            {
                return new ResponseDto(false, "OAuth ID is required for external authentication.");
            }

            var user = new Domain.Entities.User
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                LibraryID = model.LibraryID,
                Role = UserRoles.User,
                AuthProvider = model.AuthProvider,
                OAuthId = model.OAuthId ?? string.Empty,
                RegisteredAt = DateTime.UtcNow,
                IsEmailVerified = model.AuthProvider != AuthProvider.Local
            };

            if (model.AuthProvider == AuthProvider.Local)
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    return new ResponseDto(false, "Password is required for local authentication.");
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new ResponseDto(true, "User registered successfully");
        }
    }
}
