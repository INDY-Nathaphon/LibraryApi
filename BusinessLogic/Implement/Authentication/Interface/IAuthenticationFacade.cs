using LibraryApi.Controllers.DTO.AUthenticationDTO;
using LibraryApi.Controllers.DTO.BaseDTO;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Interface
{
    public interface IAuthenticationFacade
    {
        Task<ResponseDto> RegisterUserAsync(RegisterDto model);
    }
}
