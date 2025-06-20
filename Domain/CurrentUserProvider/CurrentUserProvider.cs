namespace LibraryApi.Domain.CurrentUserProvider
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private string? _userId;
        private string? _email;
        private string? _role;
        private bool _isAuthenticated;

        public string? UserId => _userId;
        public string? Email => _email;
        public string? Role => _role;
        public bool IsAuthenticated => _isAuthenticated;

        public void SetUser(string userId, string? email = null, string? role = null)
        {
            _userId = userId;
            _email = email;
            _role = role;
            _isAuthenticated = true;
        }
    }
}
