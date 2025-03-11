using Microsoft.EntityFrameworkCore;
using LibraryApi.Domain.Entities;

namespace LibraryApi.Domain
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }
        public required DbSet<Library> Libraries { get; set; }
        public required DbSet<Book> Books { get; set; }
        public required DbSet<BookFile> BookFiles { get; set; }
        public required DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // ทำให้ Email ห้ามซ้ำ

            modelBuilder.Entity<User>()
                .HasOne(u => u.Library)
                .WithMany(l => l.Users)
                .HasForeignKey(u => u.LibraryID)
                .OnDelete(DeleteBehavior.SetNull);
        }

    }
}
