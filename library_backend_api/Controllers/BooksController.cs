using library_backend_api.DTOs;
using library_backend_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace library_backend_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        // This endpoint retrieves all books. It uses caching in the service layer to improve performance.
        var books = await bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await bookService.GetBookByIdAsync(id);
        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] BookCreateDto dto)
    {
        var createdBook = await bookService.CreateBookAsync(dto);
        return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDto dto)
    {
        await bookService.UpdateBookAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        await bookService.DeleteBookAsync(id);
        return NoContent();
    }
}
