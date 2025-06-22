using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Common.Infos.Authentication
{
    public class LoginInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
