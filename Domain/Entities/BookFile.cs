namespace LibraryApi.Domain.Entities
{
    public class BookFile : BaseEntity
    {
        public long BookID { get; set; }
        public string FileType { get; set; } = string.Empty;  // เช่น "image/png", "audio/mp3"
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Book Book { get; set; } = null!;
    }
}
