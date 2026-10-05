using EBook.Domain.Entities;
using EBook.Domain.Interfaces;
using EBook.Infrastructure.Data;
using EBook.Infrastructure.ExternalServices;
using EBook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace EBook.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Detect database type from connection string
        var usePostgres = connectionString.StartsWith("Host=") || 
                          connectionString.StartsWith("Server=") ||
                          connectionString.Contains("postgres") ||
                          connectionString.Contains("DATABASE_URL");

        services.AddDbContext<AppDbContext>(options =>
        {
            if (usePostgres)
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

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
