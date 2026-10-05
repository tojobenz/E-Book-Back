namespace EBook.Application.DTOs;

public class FavoriteDto
{
    public int Id { get; set; }
    public int? BookId { get; set; }
    public string? Isbn { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public DateTime? PublishedDate { get; set; }
    public bool? IsAvailable { get; set; }
    public string? Availability { get; set; }
    public BookDto? Book { get; set; }
    public DateTime CreatedAt { get; set; }
}
