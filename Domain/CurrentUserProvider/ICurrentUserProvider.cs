namespace LibraryApi.Domain.CurrentUserProvider
{
    public interface ICurrentUserProvider
    {
        string? UserId { get; }
        string? Email { get; }
        string? Role { get; }

        bool IsAuthenticated { get; }

        void SetUser(string userId, string? email = null, string? role = null);
    }

}
