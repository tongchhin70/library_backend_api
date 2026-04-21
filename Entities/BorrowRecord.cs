namespace library_backend_api.Entities;

// Entity that tracks when a member borrows and returns a specific book.
public class BorrowRecord
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;

    // Navigation properties make it easy to include related book/member data.
    public Book? Book { get; set; }
    public Member? Member { get; set; }
}
