using EBook.Application.DTOs;
using EBook.Domain.Interfaces;
using ExternalBookDto = EBook.Domain.Interfaces.ExternalBookDto;
using Microsoft.AspNetCore.Mvc;

namespace EBook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    public const string ClientIdHeader = "X-Client-Id";

    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    /// <summary>
    /// Get favorite books for the guest client (X-Client-Id).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FavoriteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetFavorites(CancellationToken cancellationToken = default)
    {
        if (!TryGetClientId(out var clientId, out var error))
        {
            return BadRequest(error);
        }

        var favorites = await _favoriteService.GetAllAsync(clientId, cancellationToken);
        return Ok(favorites.Select(f => f.ToDto()));
    }

    /// <summary>
    /// Add a local book to favorites
    /// </summary>
    [HttpPost("local/{bookId}")]
    [ProducesResponseType(typeof(FavoriteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FavoriteDto>> AddLocalFavorite(int bookId, CancellationToken cancellationToken = default)
    {
        if (!TryGetClientId(out var clientId, out var error))
        {
            return BadRequest(error);
        }

        try
        {
            var favorite = await _favoriteService.AddAsync(clientId, bookId, cancellationToken);
            return CreatedAtAction(nameof(GetFavorites), new { }, favorite.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Add an external book (from Open Library) to favorites
    /// </summary>
    [HttpPost("external")]
    [ProducesResponseType(typeof(FavoriteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FavoriteDto>> AddExternalFavorite(
        [FromBody] ExternalBookDto externalBook,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetClientId(out var clientId, out var error))
        {
            return BadRequest(error);
        }

        try
        {
            var favorite = await _favoriteService.AddFromExternalAsync(clientId, externalBook, cancellationToken);
            return CreatedAtAction(nameof(GetFavorites), new { }, favorite.ToDto());
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Remove a local book from favorites
    /// </summary>
    [HttpDelete("local/{bookId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveLocalFavorite(int bookId, CancellationToken cancellationToken = default)
    {
        if (!TryGetClientId(out var clientId, out var error))
        {
            return BadRequest(error);
        }

        await _favoriteService.DeleteAsync(clientId, bookId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Remove an external book from favorites by ISBN
    /// </summary>
    [HttpDelete("external/{isbn}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveExternalFavorite(string isbn, CancellationToken cancellationToken = default)
    {
        if (!TryGetClientId(out var clientId, out var error))
        {
            return BadRequest(error);
        }

        await _favoriteService.DeleteByIsbnAsync(clientId, isbn, cancellationToken);
        return NoContent();
    }

    private bool TryGetClientId(out string clientId, out string error)
    {
        clientId = string.Empty;
        error = string.Empty;

        if (!Request.Headers.TryGetValue(ClientIdHeader, out var values))
        {
            error = $"{ClientIdHeader} header is required";
            return false;
        }

        clientId = values.ToString().Trim();
        if (string.IsNullOrWhiteSpace(clientId) || clientId.Length > 64)
        {
            error = $"{ClientIdHeader} must be a non-empty id (max 64 chars)";
            return false;
        }

        return true;
    }
}
