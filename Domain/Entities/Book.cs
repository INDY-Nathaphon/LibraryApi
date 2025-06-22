namespace LibraryApi.Domain.Entities
{
    public class Book : BaseEntity
    {
        public long LibraryID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime PublishedYear { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string CoverImage { get; set; } = string.Empty;
        public string AudioSample { get; set; } = string.Empty;
    }
}
