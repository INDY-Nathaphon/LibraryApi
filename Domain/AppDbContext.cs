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
        public required DbSet<Address> Addresses { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region User

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // ทำให้ Email ห้ามซ้ำ

            modelBuilder.Entity<User>()
                .HasOne(u => u.Library)
                .WithMany(l => l.Users)
                .HasForeignKey(u => u.LibraryID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Addresses)  // สำหรับ One-to-Many
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Book

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Library)
                .WithMany(l => l.Books)
                .HasForeignKey(b => b.LibraryID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.BookFiles)  // สำหรับ One-to-Many
                .WithOne(bf => bf.Book)
                .HasForeignKey(bf => bf.BookID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Loans)  // สำหรับ One-to-Many
                .WithOne(l => l.Book)
                .HasForeignKey(l => l.BookID)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region Library

            modelBuilder.Entity<Library>()
                .HasMany(l => l.Books)  // สำหรับ One-to-Many
                .WithOne(b => b.Library)
                .HasForeignKey( b => b.LibraryID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Library>()
                .HasMany(l => l.Users)
                .WithOne(u => u.Library)
                .HasForeignKey(u => u.LibraryID)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion
        }
    }
}
