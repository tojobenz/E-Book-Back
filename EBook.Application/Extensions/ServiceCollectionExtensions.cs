using EBook.Application.Services;
using EBook.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EBook.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IFavoriteService, FavoriteService>();

        return services;
    }
}
