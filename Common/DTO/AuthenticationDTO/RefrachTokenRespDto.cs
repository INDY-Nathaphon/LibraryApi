using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Common.DTO.AuthenticationDTO
{
    public class RefrachTokenRespDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
