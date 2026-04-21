using library_backend_api.Entities;

namespace library_backend_api.Repositories.Interfaces;

// Repositories handle persistence only and avoid business-rule decisions.
public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book> AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
    Task<bool> ExistsByIsbnAsync(string isbn, int? excludedId = null);
    Task<bool> TryBorrowCopyAsync(int bookId);
    Task IncrementAvailableCopiesAsync(int bookId);
}
