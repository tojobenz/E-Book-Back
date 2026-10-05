namespace EBook.Domain.Entities;

public class Favorite
{
    public int Id { get; set; }
    /// <summary>Guest browser id from localStorage (X-Client-Id). Scopes favorites without login.</summary>
    public string ClientId { get; set; } = string.Empty;
    public int? BookId { get; set; }
    public string? Isbn { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public DateTime? PublishedDate { get; set; }
    public bool? IsAvailable { get; set; }
    public string? Availability { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Book? Book { get; set; }
}
