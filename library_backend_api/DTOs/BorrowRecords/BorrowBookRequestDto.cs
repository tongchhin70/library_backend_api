using System.ComponentModel.DataAnnotations;

namespace library_backend_api.DTOs.BorrowRecords;

// Incoming payload used to borrow a book for a member.
public class BorrowBookRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "BookId must be greater than 0.")]
    public int BookId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "MemberId must be greater than 0.")]
    public int MemberId { get; set; }
}
