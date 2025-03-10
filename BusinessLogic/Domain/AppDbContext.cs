using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using LibraryApi.BusinessLogic.Domain.Entities;

namespace LibraryApi.BusinessLogic.Domain
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookFile> BookFiles { get; set; }
        public DbSet<Loan> Loans { get; set; }

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
