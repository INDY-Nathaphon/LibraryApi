using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Common.DTO.AuthenticationDTO
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // ใช้เฉพาะ Local Login
        public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;
        public string? OAuthId { get; set; } // ใช้เฉพาะ OAuth
    }
}
