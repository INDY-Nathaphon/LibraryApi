using LibraryApi.Common.Infos.Authentication;
using System.Security.Claims;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Interface
{
    public interface IAuthenticationFacade
    {
        Task Register(RegisterInfo model);

        Task<AuthResult> Login(LoginInfo model);

        Task<AuthResult> RefreshTokenAsync(string userId);

        Task<bool> RevokeRefreshToken(string refreshTokenJwt);

        Task<AuthResult> GoogleResponse(List<Claim> claims);
    }
}
