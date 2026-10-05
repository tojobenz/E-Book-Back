using EBook.Domain.Entities;
using EBook.Domain.Interfaces;
using EBook.Infrastructure.Data;

namespace EBook.Infrastructure.Repositories;

public class BookRepository : Repository<Book>
{
    public BookRepository(AppDbContext context) : base(context)
    {
    }
}
