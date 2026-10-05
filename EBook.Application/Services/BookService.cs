using EBook.Domain.Entities;
using EBook.Domain.Interfaces;

namespace EBook.Application.Services;

public class BookService : IBookService
{
    private readonly IRepository<Book> _bookRepository;

    public BookService(IRepository<Book> bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _bookRepository.GetByIdAsync(id, cancellationToken);
    }
}
