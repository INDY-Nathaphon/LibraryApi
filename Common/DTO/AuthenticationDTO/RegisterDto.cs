using static LibraryApi.Common.Enum.Enums;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Common.DTO.AuthenticationDTO
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; } // ถ้าเป็น OAuth จะไม่ต้องใช้

        [Required]
        public string Phone { get; set; } = string.Empty;

        public long? LibraryID { get; set; }

        [Required]
        public AuthProvider AuthProvider { get; set; } // Local หรือ OAuth

        public string? OAuthId { get; set; } // ใช้เฉพาะ OAuth
    }
}
