using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.Controllers.DTO.AUthenticationDTO;
using LibraryApi.Controllers.DTO.BaseDTO;
using LibraryApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static LibraryApi.BusinessLogic.Enum.Enums;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Service
{
    public class Authenticationservice : IAuthenticationService
    {

        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Domain.Entities.User> _passwordHasher;
        private readonly IConfiguration _config;
        public Authenticationservice(AppDbContext context, IPasswordHasher<Domain.Entities.User> passwordHasher, IConfiguration config) 
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        public async Task<ResponseDto> Register(RegisterDto model)
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

        public async Task<ResponseDto> Login(LoginDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null) return new ResponseDto(false, "Invalid email or password");

            // ตรวจสอบ AuthProvider (Local หรือ OAuth)
            if (model.AuthProvider == AuthProvider.Local)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Failed)
                    return new ResponseDto(false, "Invalid email or password");
            }
            else
            {
                // OAuth Login (เช็ค OAuthId)
                if (user.AuthProvider != model.AuthProvider || user.OAuthId != model.OAuthId)
                    return new ResponseDto(false, "Invalid OAuth login");
            }

            // ออก JWT Token
            string token = GenerateJwtToken(user);
            return new ResponseDto(true, "Login successful", new { token });
        }


        private string GenerateJwtToken(Domain.Entities.User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]);

            var claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                            new Claim("role", user.Role.ToString())
                        };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                _config["JwtSettings:Issuer"],
                _config["JwtSettings:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:ExpiresInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
