using System.ComponentModel.DataAnnotations;
using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Common.Infos.Authentication
{
    public class RegisterInfo
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
