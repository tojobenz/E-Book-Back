namespace EBook.Domain.Interfaces;

public interface IBookExternalService
{
    Task<IEnumerable<ExternalBookDto>> SearchBooksAsync(string query, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExternalBookDto>> GetNewReleasesAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default);
}

public class ExternalBookDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public DateTime? PublishedDate { get; set; }
    public string? Isbn { get; set; }
    public bool? IsAvailable { get; set; }
    public string? Availability { get; set; }
}
