using library_backend_api.Entities;

namespace library_backend_api.Repositories.Interfaces;

// Borrow record repository contract for database reads and writes.
public interface IBorrowRecordRepository
{
    Task<List<BorrowRecord>> GetAllAsync();
    Task<List<BorrowRecord>> GetByMemberIdAsync(int memberId);
    Task<BorrowRecord?> GetByIdAsync(int id);
    Task<BorrowRecord?> GetActiveBorrowAsync(int bookId, int memberId);
    Task<BorrowRecord> AddAsync(BorrowRecord borrowRecord);
    Task UpdateAsync(BorrowRecord borrowRecord);
    Task<int> CountActiveBorrowsForBookAsync(int bookId);
    Task<bool> HasBorrowHistoryForBookAsync(int bookId);
    Task<bool> HasBorrowHistoryForMemberAsync(int memberId);
}
