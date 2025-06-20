using LibraryApi.Common.DTO.AuthenticationDTO;
using LibraryApi.Common.DTO.BaseDTO;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Interface
{
    public interface IAuthenticationFacade
    {
        Task<ResponseDto> Register(RegisterDto model);

        Task<LoginRespDto> Login(LoginDto model);

        Task<RefrachTokenRespDto> RefreshTokenAsync( string userId, string refreshToken);
    }
}
