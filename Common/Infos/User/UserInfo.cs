using LibraryApi.Common.Infos.Base;

namespace LibraryApi.Common.Infos.User
{
    public class UserInfo : BaseInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
