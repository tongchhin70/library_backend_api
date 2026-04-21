using library_backend_api.DTOs.BorrowRecords;
using library_backend_api.Entities;
using library_backend_api.Exceptions;
using library_backend_api.Repositories.Interfaces;
using library_backend_api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace library_backend_api.Services.Implementations;

// Borrowing service coordinates inventory changes and borrow-record rules.
public class BorrowRecordService(
    IBookRepository bookRepository,
    IMemberRepository memberRepository,
    IBorrowRecordRepository borrowRecordRepository,
    IMemoryCache cache) : IBorrowRecordService
{
    public async Task<BorrowRecordResponseDto> BorrowBookAsync(BorrowBookRequestDto request)
    {
        var book = await bookRepository.GetByIdAsync(request.BookId)
            ?? throw new NotFoundException("Book not found.");

        var member = await memberRepository.GetByIdAsync(request.MemberId)
            ?? throw new NotFoundException("Member not found.");

        var existingBorrow = await borrowRecordRepository.GetActiveBorrowAsync(request.BookId, request.MemberId);
        if (existingBorrow is not null)
        {
            throw new ConflictException("This member already has an active borrow for this book.");
        }

        // This atomic update is the concurrency protection for the "last copy" scenario.
        var copyReserved = await bookRepository.TryBorrowCopyAsync(request.BookId);
        if (!copyReserved)
        {
            throw new ConflictException("No available copies remain for this book.");
        }

        try
        {
            // Only create the borrow record after inventory has been safely reserved.
            var borrowRecord = new BorrowRecord
            {
                BookId = request.BookId,
                MemberId = request.MemberId,
                BorrowDate = DateTime.UtcNow,
                Status = BorrowStatus.Borrowed
            };

            var createdRecord = await borrowRecordRepository.AddAsync(borrowRecord);
            InvalidateBookCache(request.BookId);
            return MapToResponse(createdRecord);
        }
        catch
        {
            // Roll back the reserved copy if saving the borrow record fails.
            await bookRepository.IncrementAvailableCopiesAsync(request.BookId);
            InvalidateBookCache(request.BookId);
            throw;
        }
    }

    public async Task<BorrowRecordResponseDto> ReturnBookAsync(int borrowRecordId)
    {
        var borrowRecord = await borrowRecordRepository.GetByIdAsync(borrowRecordId)
            ?? throw new NotFoundException("Borrow record not found.");

        if (borrowRecord.Status == BorrowStatus.Returned)
        {
            throw new ConflictException("This borrow record has already been returned.");
        }

        borrowRecord.Status = BorrowStatus.Returned;
        borrowRecord.ReturnDate = DateTime.UtcNow;

        // Returning first updates the record, then adds the copy back to inventory.
        await borrowRecordRepository.UpdateAsync(borrowRecord);
        await bookRepository.IncrementAvailableCopiesAsync(borrowRecord.BookId);
        InvalidateBookCache(borrowRecord.BookId);

        borrowRecord.Book = await bookRepository.GetByIdAsync(borrowRecord.BookId) switch
        {
            null => borrowRecord.Book,
            var refreshedBook => new Book
            {
                Id = refreshedBook.Id,
                Title = refreshedBook.Title,
                Author = refreshedBook.Author,
                ISBN = refreshedBook.ISBN,
                TotalCopies = refreshedBook.TotalCopies,
                AvailableCopies = refreshedBook.AvailableCopies
            }
        };

        return MapToResponse(borrowRecord);
    }

    public async Task<List<BorrowRecordResponseDto>> GetAllAsync() =>
        (await borrowRecordRepository.GetAllAsync())
        .Select(MapToResponse)
        .ToList();

    public async Task<List<BorrowRecordResponseDto>> GetMemberHistoryAsync(int memberId)
    {
        _ = await memberRepository.GetByIdAsync(memberId)
            ?? throw new NotFoundException("Member not found.");

        return (await borrowRecordRepository.GetByMemberIdAsync(memberId))
            .Select(MapToResponse)
            .ToList();
    }

    private static BorrowRecordResponseDto MapToResponse(BorrowRecord record) => new()
    {
        Id = record.Id,
        BookId = record.BookId,
        BookTitle = record.Book?.Title ?? string.Empty,
        MemberId = record.MemberId,
        MemberName = record.Member?.FullName ?? string.Empty,
        BorrowDate = record.BorrowDate,
        ReturnDate = record.ReturnDate,
        Status = record.Status.ToString()
    };

    private void InvalidateBookCache(int bookId)
    {
        // Borrowing and returning change AvailableCopies, so cached book reads must be cleared.
        cache.Remove("books:all");
        cache.Remove($"books:{bookId}");
    }
}
