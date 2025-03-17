namespace LibraryApi.Domain
{
    public class UserContext : IUserContext
    {
        private string? _userId;
        public string? UserId => _userId; // อ่านค่า UserId

        public void SetUserId(string userId) // กำหนดค่า UserId
        {
            _userId = userId;
        }
    }

    public interface IUserContext
    {
        string? UserId { get; }
        void SetUserId(string userId);
    }

}
