using library_backend_api.DTOs.BorrowRecords;

namespace library_backend_api.Services.Interfaces;

// Borrowing service contract for borrow, return, and history operations.
public interface IBorrowRecordService
{
    Task<BorrowRecordResponseDto> BorrowBookAsync(BorrowBookRequestDto request);
    Task<BorrowRecordResponseDto> ReturnBookAsync(int borrowRecordId);
    Task<List<BorrowRecordResponseDto>> GetAllAsync();
    Task<List<BorrowRecordResponseDto>> GetMemberHistoryAsync(int memberId);
}
