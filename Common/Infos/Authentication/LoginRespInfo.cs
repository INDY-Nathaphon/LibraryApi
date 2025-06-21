namespace LibraryApi.Common.Infos.Authentication
{
    public class LoginRespInfo
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
