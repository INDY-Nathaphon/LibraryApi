using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.BusinessLogic.Service.TokenBlacklist;
using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Common.Infos.Authentication;
using LibraryApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Data.Entity;
using static LibraryApi.Common.Enum.Enums;

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

        public async Task Register(RegisterInfo model)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                throw new AppException(AppErrorCode.InternalError);
            }

            if (model.AuthProvider != AuthProvider.Local && string.IsNullOrEmpty(model.OAuthId))
            {
                //return new ApiResponse(false, "OAuth ID is required for external authentication.");
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
                    //return new ApiResponse(false, "Password is required for local authentication.");
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<LoginRespInfo> Login(LoginInfo model)
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

            return new LoginRespInfo { AccessToken = accessToken, RefreshToken = refreshToken, Success = true };
        }

        public async Task<RefrachTokenRespInfo> RefreshTokenAsync(string refreshToken, string userId)
        {
            #region Validate refresh token

            _tokenService.ValidateRefreshToken(userId, refreshToken);

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

            return new RefrachTokenRespInfo { RefreshToken = newRefreshToken, AccessToken = newAccessToken, Success = true };
        }
    }
}
