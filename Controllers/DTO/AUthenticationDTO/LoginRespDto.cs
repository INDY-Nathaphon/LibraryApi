using LibraryApi.BusinessLogic.Enum;
using static LibraryApi.BusinessLogic.Enum.Enums;

namespace LibraryApi.Controllers.DTO.AUthenticationDTO
{
    public class LoginRespDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
