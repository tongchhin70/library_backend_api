using library_backend_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace library_backend_api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // These DbSets represent the main tables used by the API.
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Book rules include required fields, unique ISBN, and valid copy counts.
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(book => book.Id);
            entity.Property(book => book.Title).IsRequired();
            entity.Property(book => book.Author).IsRequired();
            entity.Property(book => book.ISBN).IsRequired();
            entity.HasIndex(book => book.ISBN).IsUnique();
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_Books_TotalCopies", "TotalCopies > 0");
                table.HasCheckConstraint("CK_Books_AvailableCopies", "AvailableCopies >= 0");
                table.HasCheckConstraint("CK_Books_AvailableCopies_Lte_TotalCopies", "AvailableCopies <= TotalCopies");
            });
        });

        // Members require a name and unique email address.
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(member => member.Id);
            entity.Property(member => member.FullName).IsRequired();
            entity.Property(member => member.Email).IsRequired();
            entity.HasIndex(member => member.Email).IsUnique();
        });

        // Borrow records connect members and books and store their current status.
        modelBuilder.Entity<BorrowRecord>(entity =>
        {
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Status)
                .HasConversion<string>()
                .IsRequired();

            entity.HasOne(record => record.Book)
                .WithMany(book => book.BorrowRecords)
                .HasForeignKey(record => record.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(record => record.Member)
                .WithMany(member => member.BorrowRecords)
                .HasForeignKey(record => record.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(record => new { record.MemberId, record.BookId, record.Status });
        });
    }
}
