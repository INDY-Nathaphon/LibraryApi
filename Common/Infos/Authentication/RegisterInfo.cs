using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Common.Infos.Authentication
{
    public class RegisterInfo
    {
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}
