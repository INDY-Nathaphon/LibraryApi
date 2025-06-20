using System.Security.Cryptography;
using LibraryApi.Domain.Entities;

namespace LibraryApi.BusinessLogic.Service.TokenBlacklist
{
    public interface ITokenService
    {
        public string GenerateAccessToken(User user);
        public void StoreRefreshToken(string userId, string refreshToken);
        public void ValidateRefreshToken(string userId, string refreshToken);
        public void RevokeRefreshToken(string userId);
        public string GenerateRefreshToken();
        public string GetUserIdFromRefreshToken(string refreshToken);
        public string? ValidateAccessToken(string token);
    }
}
