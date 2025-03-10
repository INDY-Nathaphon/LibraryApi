namespace LibraryApi.BusinessLogic.Domain.Entities
{
    public class Library : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public ICollection<User> Members { get; set; } = new List<User>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
