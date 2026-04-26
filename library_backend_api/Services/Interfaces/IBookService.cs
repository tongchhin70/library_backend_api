using library_backend_api.DTOs;

namespace library_backend_api.Services.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookResponseDto>> GetAllBooksAsync();
    Task<BookResponseDto> GetBookByIdAsync(int id);
    Task<BookResponseDto> CreateBookAsync(BookCreateDto dto);
    Task UpdateBookAsync(int id, BookUpdateDto dto);
    Task DeleteBookAsync(int id);
}
