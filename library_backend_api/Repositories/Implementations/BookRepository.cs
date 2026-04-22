using library_backend_api.Data;
using library_backend_api.Entities;
using library_backend_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace library_backend_api.Repositories.Implementations;

public class BookRepository(AppDbContext context) : IBookRepository
{
    public Task<List<Book>> GetAllAsync() =>
        context.Books.AsNoTracking().OrderBy(book => book.Id).ToListAsync();

    public Task<Book?> GetByIdAsync(int id) =>
        context.Books.AsNoTracking().FirstOrDefaultAsync(book => book.Id == id);

    public async Task<Book> AddAsync(Book book)
    {
        context.Books.Add(book);
        await context.SaveChangesAsync();
        return book;
    }

    public async Task UpdateAsync(Book book)
    {
        context.Books.Update(book);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        context.Books.Remove(book);
        await context.SaveChangesAsync();
    }

    public Task<bool> ExistsByIsbnAsync(string isbn, int? excludedId = null) =>
        context.Books.AnyAsync(book => book.ISBN == isbn && (!excludedId.HasValue || book.Id != excludedId.Value));

    public async Task<bool> TryBorrowCopyAsync(int bookId)
    {
        var affectedRows = await context.Database.ExecuteSqlInterpolatedAsync($@"
UPDATE Books
SET AvailableCopies = AvailableCopies - 1
WHERE Id = {bookId} AND AvailableCopies > 0");

        return affectedRows == 1;
    }

    public Task IncrementAvailableCopiesAsync(int bookId) =>
        context.Database.ExecuteSqlInterpolatedAsync($@"
UPDATE Books
SET AvailableCopies = AvailableCopies + 1
WHERE Id = {bookId}");
}
