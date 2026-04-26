using library_backend_api.DTOs;
using library_backend_api.Entities;
using library_backend_api.Exceptions;
using library_backend_api.Repositories.Interfaces;
using library_backend_api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace library_backend_api.Services.Implementations;

public class BookService(IBookRepository repository, IMemoryCache cache) : IBookService
{
    private const string AllBooksCacheKey = "AllBooksCache";

    public async Task<IEnumerable<BookResponseDto>> GetAllBooksAsync()
    {
        if (cache.TryGetValue(AllBooksCacheKey, out IEnumerable<BookResponseDto>? cachedBooks))
        {
            return cachedBooks!;
        }

        var books = await repository.GetAllAsync();
        var dtos = books.Select(MapToDto).ToList();

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        
        cache.Set(AllBooksCacheKey, dtos, cacheOptions);

        return dtos;
    }

    public async Task<BookResponseDto> GetBookByIdAsync(int id)
    {
        var book = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        return MapToDto(book);
    }

    public async Task<BookResponseDto> CreateBookAsync(BookCreateDto dto)
    {
        if (await repository.ExistsByIsbnAsync(dto.ISBN))
        {
            //duplicate ISBN is not a server crash. It is a conflict with existing data.
            throw new ConflictException($"A book with ISBN {dto.ISBN} already exists.");
        }

        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            ISBN = dto.ISBN,
            TotalCopies = dto.TotalCopies,
            AvailableCopies = dto.TotalCopies
        };

        var createdBook = await repository.AddAsync(book);
        
        // INVALIDATE CACHE
        cache.Remove(AllBooksCacheKey);

        return MapToDto(createdBook);
    }

    public async Task UpdateBookAsync(int id, BookUpdateDto dto)
    {
        var existingBook = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        if (await repository.ExistsByIsbnAsync(dto.ISBN, id))
        {
            throw new ConflictException($"Another book with ISBN {dto.ISBN} already exists.");
        }

        existingBook.Title = dto.Title;
        existingBook.Author = dto.Author;
        existingBook.ISBN = dto.ISBN;
        
        // Update TotalCopies
        existingBook.TotalCopies = dto.TotalCopies; 

        await repository.UpdateAsync(existingBook);
        
        // INVALIDATE CACHE
        cache.Remove(AllBooksCacheKey);
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Book with ID {id} not found.");

        await repository.DeleteAsync(book);
        
        // INVALIDATE CACHE
        cache.Remove(AllBooksCacheKey);
    }

    private static BookResponseDto MapToDto(Book book)
    {
        return new BookResponseDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies
        };
    }

}
