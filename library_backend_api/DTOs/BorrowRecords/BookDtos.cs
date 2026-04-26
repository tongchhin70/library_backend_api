namespace library_backend_api.DTOs;
using System.ComponentModel.DataAnnotations;


public class BookResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}

public class BookCreateDto
{
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Author is required.")]
    public string Author { get; set; } = string.Empty;
    [Required(ErrorMessage = "ISBN is required.")]
    public string ISBN { get; set; } = string.Empty;
    [Range(1, int.MaxValue, ErrorMessage = "TotalCopies must be greater than 0.")]
    public int TotalCopies { get; set; }
}

public class BookUpdateDto
{
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Author is required.")]
    public string Author { get; set; } = string.Empty;
    [Required(ErrorMessage = "ISBN is required.")]
    public string ISBN { get; set; } = string.Empty;
    [Range(1, int.MaxValue, ErrorMessage = "TotalCopies must be greater than 0.")]
    public int TotalCopies { get; set; }
}