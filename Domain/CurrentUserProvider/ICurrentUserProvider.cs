namespace LibraryApi.Domain.CurrentUserProvider
{
    public interface ICurrentUserProvider
    {
        long UserId { get; }
        string? Email { get; }
        string? Role { get; }

        bool IsAuthenticated { get; }

        void SetUser(long userId, string? email = null, string? role = null);
    }

}
