namespace LibraryApi.Common.Enum
{
    public class Enums
    {
        public enum UserRoles
        {
            Admin = 1,
            User = 2,
            Guest = 3,
            Librarian = 4,
            Manager = 5
        }

        public enum AuthProvider
        {
            Local = 1,
            Google = 2,
        }

        public enum LibraryPolicy
        {
            Read,
            Write,
            Admin
        }

        public enum LibraryPermission
        {
            Read,
            Write,
            Admin
        }

    }
}
