using LibraryApi.Controllers.DTO.AUthenticationDTO;
using LibraryApi.Controllers.DTO.BaseDTO;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Interface
{
    public interface IAuthenticationFacade
    {
        Task<ResponseDto> Register(RegisterDto model);

        Task<LoginRespDto> Login(LoginDto model);

        Task<RefrachTokenRespDto> RefreshTokenAsync( string userId, string refreshToken);
    }
}
