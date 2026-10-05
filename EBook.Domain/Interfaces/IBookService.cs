using EBook.Domain.Entities;

namespace EBook.Domain.Interfaces;

public interface IBookService
{
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
