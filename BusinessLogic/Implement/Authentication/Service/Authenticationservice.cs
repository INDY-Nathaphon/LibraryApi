using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using Azure;
using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static LibraryApi.Common.Enum.Enums;
using LibraryApi.BusinessLogic.Service.TokenBlacklist;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore.Storage;
using LibraryApi.BusinessLogic.Implement.User.Interface;
using StackExchange.Redis;
using LibraryApi.Common.DTO.AuthenticationDTO;
using LibraryApi.Common.DTO.BaseDTO;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Domain.Entities.User> _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IDistributedCache _redis; // ใช้ IDistributedCache ที่มาจาก DI
        private readonly IUserService _userService;

        public AuthenticationService(
            AppDbContext context,
            IPasswordHasher<Domain.Entities.User> passwordHasher,
            ITokenService tokenService,
            IDistributedCache redis,  // รับจาก DI
            IUserService userService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _redis = redis; // ไม่ต้องสร้าง ConnectionMultiplexer เอง
            _userService = userService;
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

        public async Task<LoginRespDto> Login(LoginDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null) throw new Exception("User not found");

            // ตรวจสอบ AuthProvider (Local หรือ OAuth)
            if (model.AuthProvider == AuthProvider.Local)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Failed)
                    throw new Exception("Invalid password");
            }
            else
            {
                // OAuth Login (เช็ค OAuthId)
                if (user.AuthProvider != model.AuthProvider || user.OAuthId != model.OAuthId)
                    throw new Exception("Invalid OAuth credentials");
            }

            // ออก JWT Token
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            #region Store Refresh Token in Redis

            _tokenService.StoreRefreshToken(user.Id.ToString(), refreshToken); // StoreRefreshToken

            #endregion

            return new LoginRespDto { AccessToken = accessToken, RefreshToken = refreshToken, Success   = true };
        }

        public async Task<RefrachTokenRespDto> RefreshTokenAsync(string refreshToken, string userId)
        {
            #region Validate refresh token

            _tokenService.ValidateRefreshToken(userId,refreshToken);

            #endregion

            var user = await _userService.GetByIdAsync(Convert.ToInt32(userId));

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Update Refresh Token in Redis
            await _redis.SetStringAsync(user.Id.ToString(), newRefreshToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            });

            return new RefrachTokenRespDto { RefreshToken = newRefreshToken, AccessToken = newAccessToken, Success = true };
        }
    }
}
