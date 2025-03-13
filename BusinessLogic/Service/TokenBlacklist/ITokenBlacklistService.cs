namespace LibraryApi.BusinessLogic.Service.TokenBlacklist
{
    public interface ITokenBlacklistService
    {
        Task RevokeTokenAsync(string token, int expiresInMinutes);
        Task<bool> IsTokenRevokedAsync(string token);
    }

}
