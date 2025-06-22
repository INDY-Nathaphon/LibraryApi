namespace LibraryApi.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public long LoanID { get; set; }
        public long MemberID { get; set; }
        public long BookID { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }
        public User Member { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}
