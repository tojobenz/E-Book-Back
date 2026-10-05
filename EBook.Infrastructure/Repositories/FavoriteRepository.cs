using EBook.Domain.Entities;
using EBook.Domain.Interfaces;
using EBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EBook.Infrastructure.Repositories;

public class FavoriteRepository : Repository<Favorite>
{
    public FavoriteRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Favorite>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Book)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
