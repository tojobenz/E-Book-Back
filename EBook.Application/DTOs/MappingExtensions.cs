using EBook.Domain.Entities;

namespace EBook.Application.DTOs;

public static class MappingExtensions
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            CoverUrl = book.CoverUrl,
            PublishedDate = book.PublishedDate,
            Isbn = book.Isbn,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt
        };
    }

    public static FavoriteDto ToDto(this Favorite favorite)
    {
        return new FavoriteDto
        {
            Id = favorite.Id,
            BookId = favorite.BookId,
            Isbn = favorite.Isbn,
            Title = favorite.Title,
            Author = favorite.Author,
            Description = favorite.Description,
            CoverUrl = favorite.CoverUrl,
            PublishedDate = favorite.PublishedDate,
            IsAvailable = favorite.IsAvailable,
            Availability = favorite.Availability,
            Book = favorite.Book?.ToDto(),
            CreatedAt = favorite.CreatedAt
        };
    }
}
