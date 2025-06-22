using LibraryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Domain
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public required DbSet<User> Users { get; set; }
        public required DbSet<Library> Libraries { get; set; }
        public required DbSet<Book> Books { get; set; }
        public required DbSet<BookFile> BookFiles { get; set; }
        public required DbSet<Loan> Loans { get; set; }
        public required DbSet<Address> Addresses { get; set; }
        public required DbSet<UserLibrary> UserLibraries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
