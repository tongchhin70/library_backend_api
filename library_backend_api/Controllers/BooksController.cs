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
        var books = await bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await bookService.GetBookByIdAsync(id);
        if (book == null) return NotFound(new { error = $"Book with ID {id} not found." });
        
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
        var updated = await bookService.UpdateBookAsync(id, dto);
        if (!updated) return NotFound(new { error = $"Book with ID {id} not found." });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var deleted = await bookService.DeleteBookAsync(id);
        if (!deleted) return NotFound(new { error = $"Book with ID {id} not found." });

        return NoContent();
    }
}