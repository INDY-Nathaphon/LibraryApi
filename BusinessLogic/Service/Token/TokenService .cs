using LibraryApi.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using LibraryApi.Domain;
using Microsoft.Extensions.Options;

namespace LibraryApi.BusinessLogic.Service.TokenBlacklist
{
    public class TokenService : ITokenService
    {
        private readonly IDistributedCache _redis;
        private readonly AppSettings _appSettings;

        public TokenService(IDistributedCache redis, IOptions<AppSettings> appSettings)
        {
            _redis = redis;
            _appSettings = appSettings.Value;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSetting.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _appSettings.JwtSetting.Issuer,
                audience: _appSettings.JwtSetting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), // ✅ Access Token อายุ 15 นาที
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async void StoreRefreshToken(string userId, string refreshToken)
        {
            await _redis.SetStringAsync($"refresh:{userId}", refreshToken);
        }

        public async void ValidateRefreshToken(string userId, string refreshToken)
        {
            var storedToken = await _redis.GetStringAsync($"refresh:{userId}");

            if (storedToken != refreshToken)
            {
                throw new Exception("Invalid refresh token.");
            }
        }

        public void RevokeRefreshToken(string userId)
        {
            _redis.RemoveAsync($"refresh:{userId}"); // ❌ ลบ Refresh Token ออกจาก Redis
        }

        public string GetUserIdFromRefreshToken(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _appSettings.JwtSetting.Secret;

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty.");
            }

            var key = Encoding.ASCII.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ReadToken(refreshToken) as JwtSecurityToken;
                if (principal != null)
                {
                    var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    return userId ?? string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        public string? ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.JwtSetting.Secret);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _appSettings.JwtSetting.Issuer,
                    ValidAudience = _appSettings.JwtSetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // ตรวจสอบว่า token เป็น JWT และใช้ Algorithm ที่ถูกต้อง
                if (validatedToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    return userIdClaim?.Value; // คืนค่า userId กลับไป
                }
            }
            catch (Exception)
            {
                return null; // คืนค่า null หาก token ไม่ถูกต้อง
            }

            return null;
        }

    }

}
