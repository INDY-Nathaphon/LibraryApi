using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.BusinessLogic.Infrastructure.Redis;
using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Common.Infos.Authentication;
using LibraryApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Domain.Entities.User> _passwordHasher;
        private readonly IRedisService _redisService;
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;


        public AuthenticationService(
            AppDbContext context,
            IPasswordHasher<Domain.Entities.User> passwordHasher,
            IRedisService redis,
            AppSettings appSettings,
            IUserService userService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _redisService = redis;
            _userService = userService;
            _appSettings = appSettings;
        }

        public async Task Register(RegisterInfo model)
        {
            #region Validate model

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new AppException(AppErrorCode.ValidationError, "Name is required.");
            }

            if (string.IsNullOrEmpty(model.Phone))
            {
                throw new AppException(AppErrorCode.ValidationError, "Phone is required.");
            }

            if (model.Phone.Length != 10)
            {
                throw new AppException(AppErrorCode.ValidationError, "Phone number must be 10 digits.");
            }

            if (!Regex.IsMatch(model.Phone, @"^(06|08|09)\d{8}$"))
            {
                throw new AppException(AppErrorCode.ValidationError, "Phone number must start with 06, 08, or 09.");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                throw new AppException(AppErrorCode.ValidationError, "Password is required for local authentication.");
            }

            #endregion

            var currentDate = DateTime.UtcNow;

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email || u.Phone == model.Phone);

            if (existingUser != null)
            {
                throw new AppException(AppErrorCode.ValidationError, "Email or phone already exists.");
            }

            var user = new Domain.Entities.User
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Role = UserRoles.User,
                AuthProvider = AuthProvider.Local,
                RegisteredAt = currentDate,
                IsEmailVerified = true,
                CreatedDate = currentDate,
                UpdatedDate = currentDate
            };


            if (string.IsNullOrEmpty(model.Password))
            {
                throw new AppException(AppErrorCode.ValidationError, "Password is required for local authentication.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResult> Login(LoginInfo model)
        {
            #region Validate model

            if (string.IsNullOrEmpty(model.Email))
            {
                throw new AppException(AppErrorCode.ValidationError, "Email is required.");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                throw new AppException(AppErrorCode.ValidationError, "Password is required for local authentication.");
            }

            #endregion

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                throw new AppException(AppErrorCode.NotFound, "User not found.");
            }

            if (!user.IsEmailVerified)
            {
                throw new AppException(AppErrorCode.ValidationError, "Email is not verified.");
            }

            if (user.AuthProvider != AuthProvider.Local)
            {
                throw new AppException(AppErrorCode.Unauthorized, "Invalid authentication.");
            }

            #region Validate password

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new AppException(AppErrorCode.ValidationError, "Invalid password.");
            }

            #endregion

            var resultAuth = await GetAuthResultAsync(user);

            return resultAuth;
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            #region Validate refresh token

            var userId = await ValidateRefreshTokenAsync(refreshToken);

            #endregion

            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                throw new AppException(AppErrorCode.NotFound, "User not found.");
            }

            var resultAuth = await GetAuthResultAsync(user);

            return resultAuth;
        }

        public async Task<bool> RevokeRefreshToken(string refreshTokenJwt)
        {
            // 1. Validate Refresh Token JWT เพื่อดึง JTI
            var principal = GetPrincipalFromRefreshToken(refreshTokenJwt);
            if (principal == null)
            {
                return false;
            }

            var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var userIdString = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (jti == null || userIdString == null || !int.TryParse(userIdString, out int userId))
            {
                return false;
            }

            // 2. ลบ Key ของ Refresh Token นั้นออกจาก Redis
            var isDeleted = await _redisService.KeyDeleteAsync(jti);

            // 3. Optional: บันทึกการ Revoke ลงในฐานข้อมูลเพื่อ Audit
            // if (isDeleted) { await RecordRevokedRefreshToken(jti, userId, GetIpAddress()); }
            return isDeleted;
        }

        public async Task<AuthResult> GoogleResponse(List<Claim> claims)
        {
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var oauthId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (email == null || oauthId == null)
            {
                throw new AppException(AppErrorCode.Unauthorized, "Invalid authentication.");
            }

            var user = await (from u in _context.Users
                              where u.Email == email && u.OAuthId == oauthId
                              select u).FirstOrDefaultAsync();
            if (user == null)
            {
                user = new Domain.Entities.User
                {
                    Email = email,
                    OAuthId = oauthId,
                    Name = name ?? email,
                    AuthProvider = AuthProvider.Google,
                    Role = UserRoles.User,
                    IsEmailVerified = true
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            var resultAuth = await GetAuthResultAsync(user);

            return resultAuth;
        }

        #region Private

        private string GenerateAccessToken(Domain.Entities.User user)
        {
            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, user.Role.ToString())
                        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _appSettings.JwtSettings.Issuer,
                audience: _appSettings.JwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_appSettings.JwtSettings.ExpiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(Domain.Entities.User user)
        {
            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.Role, user.Role.ToString()),
                                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim("typ", "refresh") // ✅ ระบุว่าเป็น Refresh Token
                            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSettings.RefreshTokenSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _appSettings.JwtSettings.Issuer,
                audience: _appSettings.JwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_appSettings.JwtSettings.RefreshTokenExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<AuthResult> GetAuthResultAsync(Domain.Entities.User user)
        {
            // ออก JWT Token
            var accessToken = GenerateAccessToken(user);
            var refreshTokenJwt = GenerateRefreshToken(user);

            // 4. Extract JTI และ Expiry จาก Refresh Token JWT
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(refreshTokenJwt) as JwtSecurityToken;
            var jti = jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiryTime = jsonToken?.ValidTo;

            if (jti == null || expiryTime == null)
            {
                throw new AppException(AppErrorCode.Unauthorized, "Invalid refresh token.");
            }

            // 5. เก็บ Refresh Token JTI ใน Redis พร้อม User ID และ TTL
            await _redisService.SetAsync(jti, user.Id.ToString(), expiryTime - DateTime.UtcNow);

            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenJwt
            };
        }

        private ClaimsPrincipal? GetPrincipalFromRefreshToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.JwtSettings.RefreshTokenSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = _appSettings.JwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _appSettings.JwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                return null; // Refresh Token หมดอายุ
            }
            catch (Exception)
            {
                return null; // Invalid token
            }
        }

        private async Task<long> ValidateRefreshTokenAsync(string refreshTokenJwt)
        {
            var principal = GetPrincipalFromRefreshToken(refreshTokenJwt);

            if (principal == null)
            {
                throw new AppException(AppErrorCode.Unauthorized, "Invalid Refresh Token.");
            }

            var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var userIdString = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (jti == null || userIdString == null || !int.TryParse(userIdString, out int userId))
            {
                throw new AppException(AppErrorCode.Unauthorized, "Invalid Refresh Token.");
            }

            var redisValue = await _redisService.GetAsync(jti);

            if (redisValue == null || redisValue != userId.ToString())
            {
                throw new AppException(AppErrorCode.Unauthorized, "Refresh Token not store on redis.");
            }

            // 3. (Best Practice) Implement Refresh Token Rotation: Revoke Token เก่า
            await _redisService.KeyDeleteAsync(jti);

            return userId;
        }

        #endregion
    }
}
