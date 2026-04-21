using library_backend_api.Data;
using library_backend_api.Entities;
using library_backend_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace library_backend_api.Repositories.Implementations;

// Borrow record repository handles queries for borrow history and active loans.
public class BorrowRecordRepository(AppDbContext context) : IBorrowRecordRepository
{
    public Task<List<BorrowRecord>> GetAllAsync() =>
        context.BorrowRecords
            .AsNoTracking()
            .Include(record => record.Book)
            .Include(record => record.Member)
            .OrderByDescending(record => record.BorrowDate)
            .ToListAsync();

    public Task<List<BorrowRecord>> GetByMemberIdAsync(int memberId) =>
        context.BorrowRecords
            .AsNoTracking()
            .Include(record => record.Book)
            .Include(record => record.Member)
            .Where(record => record.MemberId == memberId)
            .OrderByDescending(record => record.BorrowDate)
            .ToListAsync();

    public Task<BorrowRecord?> GetByIdAsync(int id) =>
        context.BorrowRecords
            .Include(record => record.Book)
            .Include(record => record.Member)
            .FirstOrDefaultAsync(record => record.Id == id);

    public Task<BorrowRecord?> GetActiveBorrowAsync(int bookId, int memberId) =>
        // Used to prevent borrowing the same book twice before it is returned.
        context.BorrowRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(record =>
                record.BookId == bookId &&
                record.MemberId == memberId &&
                record.Status == BorrowStatus.Borrowed);

    public async Task<BorrowRecord> AddAsync(BorrowRecord borrowRecord)
    {
        context.BorrowRecords.Add(borrowRecord);
        await context.SaveChangesAsync();
        // Load related entities so the response DTO can include names/titles.
        await context.Entry(borrowRecord).Reference(record => record.Book).LoadAsync();
        await context.Entry(borrowRecord).Reference(record => record.Member).LoadAsync();
        return borrowRecord;
    }

    public async Task UpdateAsync(BorrowRecord borrowRecord)
    {
        context.BorrowRecords.Update(borrowRecord);
        await context.SaveChangesAsync();
    }

    public Task<int> CountActiveBorrowsForBookAsync(int bookId) =>
        context.BorrowRecords.CountAsync(record => record.BookId == bookId && record.Status == BorrowStatus.Borrowed);

    public Task<bool> HasBorrowHistoryForBookAsync(int bookId) =>
        context.BorrowRecords.AnyAsync(record => record.BookId == bookId);

    public Task<bool> HasBorrowHistoryForMemberAsync(int memberId) =>
        context.BorrowRecords.AnyAsync(record => record.MemberId == memberId);
}
