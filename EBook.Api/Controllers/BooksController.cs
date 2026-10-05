using EBook.Application.DTOs;
using EBook.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EBook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Get a book by ID
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <returns>Book details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBook(int id, CancellationToken cancellationToken = default)
    {
        var book = await _bookService.GetByIdAsync(id, cancellationToken);
        if (book == null)
        {
            return NotFound();
        }

        return Ok(book.ToDto());
    }
}
