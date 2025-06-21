using LibraryApi.Common.Infos.Authentication;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Interface
{
    public interface IAuthenticationFacade
    {
        Task Register(RegisterInfo model);

        Task<LoginRespInfo> Login(LoginInfo model);

        Task<RefrachTokenRespInfo> RefreshTokenAsync(string userId, string refreshToken);
    }
}
