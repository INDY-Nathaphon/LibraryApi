namespace LibraryApi.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public int LoanID { get; set; }
        public int MemberID { get; set; }
        public int BookID { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }
        public User Member { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}
