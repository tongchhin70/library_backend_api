using library_backend_api.DTOs;

namespace library_backend_api.Services.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookResponseDto>> GetAllBooksAsync();
    Task<BookResponseDto?> GetBookByIdAsync(int id);
    Task<BookResponseDto> CreateBookAsync(BookCreateDto dto);
    Task<bool> UpdateBookAsync(int id, BookUpdateDto dto);
    Task<bool> DeleteBookAsync(int id);
}