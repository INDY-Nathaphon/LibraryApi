using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Hash ไว้ถ้าใช้ระบบ Local
        public UserRoles Role { get; set; } = UserRoles.User;
        public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;
        public string OAuthId { get; set; } = string.Empty; // ID จาก Google/Facebook
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public bool IsEmailVerified { get; set; } = false; // ใช้เช็คว่าผ่าน Email Verification หรือยัง
    }
}
