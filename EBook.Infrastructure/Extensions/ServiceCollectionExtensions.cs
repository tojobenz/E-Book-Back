using EBook.Domain.Entities;
using EBook.Domain.Interfaces;
using EBook.Infrastructure.Data;
using EBook.Infrastructure.ExternalServices;
using EBook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EBook.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IRepository<Book>, BookRepository>();
        services.AddScoped<IRepository<Favorite>, FavoriteRepository>();
        
        services.AddHttpClient<IBookExternalService, BookExternalService>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("EBookApp/1.0 (portfolio project)");
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        return services;
    }
}
