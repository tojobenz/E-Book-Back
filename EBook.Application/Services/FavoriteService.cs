using EBook.Domain.Entities;
using EBook.Domain.Interfaces;

namespace EBook.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IRepository<Favorite> _favoriteRepository;
    private readonly IRepository<Book> _bookRepository;

    public FavoriteService(IRepository<Favorite> favoriteRepository, IRepository<Book> bookRepository)
    {
        _favoriteRepository = favoriteRepository;
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Favorite>> GetAllAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var all = await _favoriteRepository.GetAllAsync(cancellationToken);
        return all.Where(f => f.ClientId == clientId);
    }

    public async Task<Favorite> AddAsync(string clientId, int bookId, CancellationToken cancellationToken = default)
    {
        var existing = (await GetAllAsync(clientId, cancellationToken))
            .FirstOrDefault(f => f.BookId == bookId);

        if (existing != null)
        {
            return existing;
        }

        var book = await _bookRepository.GetByIdAsync(bookId, cancellationToken);
        if (book == null)
        {
            throw new InvalidOperationException($"Book with ID {bookId} not found");
        }

        var favorite = new Favorite
        {
            ClientId = clientId,
            BookId = bookId,
            CreatedAt = DateTime.UtcNow
        };

        var added = await _favoriteRepository.AddAsync(favorite, cancellationToken);
        added.Book = book;
        return added;
    }

    public async Task<Favorite> AddFromExternalAsync(
        string clientId,
        ExternalBookDto externalBook,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalBook.Isbn))
        {
            throw new InvalidOperationException("ISBN is required for external book favorites");
        }

        var existing = (await GetAllAsync(clientId, cancellationToken))
            .FirstOrDefault(f => f.Isbn == externalBook.Isbn);

        if (existing != null)
        {
            return existing;
        }

        var favorite = new Favorite
        {
            ClientId = clientId,
            Isbn = externalBook.Isbn,
            Title = externalBook.Title,
            Author = externalBook.Author,
            Description = externalBook.Description,
            CoverUrl = externalBook.CoverUrl,
            PublishedDate = externalBook.PublishedDate,
            IsAvailable = externalBook.IsAvailable,
            Availability = externalBook.Availability,
            CreatedAt = DateTime.UtcNow
        };

        return await _favoriteRepository.AddAsync(favorite, cancellationToken);
    }

    public async Task DeleteAsync(string clientId, int bookId, CancellationToken cancellationToken = default)
    {
        var favorite = (await GetAllAsync(clientId, cancellationToken))
            .FirstOrDefault(f => f.BookId == bookId);

        if (favorite != null)
        {
            await _favoriteRepository.DeleteAsync(favorite.Id, cancellationToken);
        }
    }

    public async Task DeleteByIsbnAsync(string clientId, string isbn, CancellationToken cancellationToken = default)
    {
        var favorite = (await GetAllAsync(clientId, cancellationToken))
            .FirstOrDefault(f => f.Isbn == isbn);

        if (favorite != null)
        {
            await _favoriteRepository.DeleteAsync(favorite.Id, cancellationToken);
        }
    }
}
