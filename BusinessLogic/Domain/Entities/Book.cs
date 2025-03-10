namespace LibraryApi.BusinessLogic.Domain.Entities
{
    public class Book : BaseEntity
    {
        public long LibraryID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string CoverImage { get; set; } = string.Empty;
        public string AudioSample { get; set; } = string.Empty;

        public Library Library { get; set; } = null!;
        public ICollection<BookFile> BookFiles { get; set; } = new List<BookFile>();
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
