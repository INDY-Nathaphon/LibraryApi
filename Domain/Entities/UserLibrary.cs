namespace LibraryApi.Domain.Entities
{
    public class UserLibrary : BaseEntity
    {
        public long UserId { get; set; }
        public long LibraryId { get; set; }
    }
}
