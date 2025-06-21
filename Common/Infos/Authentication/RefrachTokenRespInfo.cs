namespace LibraryApi.Common.Infos.Authentication
{
    public class RefrachTokenRespInfo
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
