using LibraryApi.Controllers.DTO.AUthenticationDTO;
using LibraryApi.Controllers.DTO.BaseDTO;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Interface
{
    public interface IAuthenticationFacade
    {
        Task<ResponseDto> Register(RegisterDto model);

        Task<ResponseDto> Login(LoginDto model);
    }
}
