﻿namespace LibraryApi.Common.Infos.Authentication
{
    public class AuthResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}