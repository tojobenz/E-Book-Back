using EBook.Domain.Interfaces;
using ExternalBookDto = EBook.Domain.Interfaces.ExternalBookDto;
using Microsoft.AspNetCore.Mvc;

namespace EBook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExternalController : ControllerBase
{
    private readonly IBookExternalService _bookExternalService;

    public ExternalController(IBookExternalService bookExternalService)
    {
        _bookExternalService = bookExternalService;
    }

    /// <summary>
    /// Search books from external API (Open Library)
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ExternalBookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExternalBookDto>>> SearchBooks(
        [FromQuery] string query,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter is required");
        }

        var books = await _bookExternalService.SearchBooksAsync(query, limit, offset, cancellationToken);
        return Ok(books.ToList());
    }

    /// <summary>
    /// Get recent new releases from Open Library
    /// </summary>
    [HttpGet("new")]
    [ProducesResponseType(typeof(IEnumerable<ExternalBookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExternalBookDto>>> GetNewReleases(
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        var books = await _bookExternalService.GetNewReleasesAsync(limit, offset, cancellationToken);
        return Ok(books.ToList());
    }
}
