using EBook.Domain.Entities;

namespace EBook.Domain.Interfaces;

public interface IFavoriteService
{
    Task<IEnumerable<Favorite>> GetAllAsync(string clientId, CancellationToken cancellationToken = default);
    Task<Favorite> AddAsync(string clientId, int bookId, CancellationToken cancellationToken = default);
    Task<Favorite> AddFromExternalAsync(string clientId, ExternalBookDto externalBook, CancellationToken cancellationToken = default);
    Task DeleteAsync(string clientId, int bookId, CancellationToken cancellationToken = default);
    Task DeleteByIsbnAsync(string clientId, string isbn, CancellationToken cancellationToken = default);
}
