using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Domain.Entities
{
    public class User : BaseEntity
    {
        public long? LibraryID { get; set; } // ถ้าเป็น SuperAdmin อาจไม่มี Library
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Hash ไว้ถ้าใช้ระบบ Local
        public UserRoles Role { get; set; } = UserRoles.User;
        public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;
        public string OAuthId { get; set; } = string.Empty; // ID จาก Google/Facebook
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public bool IsEmailVerified { get; set; } = false; // ใช้เช็คว่าผ่าน Email Verification หรือยัง

        // Navigation Properties
        public Library? Library { get; set; } // อาจเป็น null ถ้าเป็น SuperAdmin
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
