using System.Text.Json;
using System.Text.Json.Serialization;
using EBook.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EBook.Infrastructure.ExternalServices;

public class BookExternalService(HttpClient httpClient, ILogger<BookExternalService> logger) : IBookExternalService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<BookExternalService> _logger = logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string SearchFields = "title,author_name,isbn,cover_i,first_publish_year,edition_key,availability";

    public async Task<IEnumerable<ExternalBookDto>> SearchBooksAsync(
        string query,
        int limit = 10,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 1, 40);
        offset = Math.Max(0, offset);
        // Small over-fetch to compensate for docs without ISBN
        var fetchLimit = Math.Min(limit + 15, 50);
        var url = $"https://openlibrary.org/search.json?q={Uri.EscapeDataString(query)}&fields={SearchFields}&limit={fetchLimit}&offset={offset}";
        return await FetchBooksAsync(url, cancellationToken, take: limit);
    }

    public async Task<IEnumerable<ExternalBookDto>> GetNewReleasesAsync(
        int limit = 10,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 1, 40);
        offset = Math.Max(0, offset);
        var currentYear = DateTime.UtcNow.Year;
        var query = $"first_publish_year:[{currentYear - 1} TO {currentYear}]";
        // Over-fetch so quality filter still yields ~limit items per page
        var fetchLimit = Math.Min(limit * 3, 60);
        var url = $"https://openlibrary.org/search.json?q={Uri.EscapeDataString(query)}&sort=new&fields={SearchFields}&limit={fetchLimit}&offset={offset}";
        return await FetchBooksAsync(url, cancellationToken, take: limit, preferQuality: true);
    }

    private async Task<IEnumerable<ExternalBookDto>> FetchBooksAsync(
        string url,
        CancellationToken cancellationToken,
        int take,
        bool preferQuality = false)
    {
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch books from Open Library. Status: {StatusCode}", response.StatusCode);
                return Enumerable.Empty<ExternalBookDto>();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var searchResult = JsonSerializer.Deserialize<OpenLibrarySearchResponse>(content, JsonOptions);

            var books = searchResult?.Docs?.Select(MapToDto)
                            .Where(b => !string.IsNullOrWhiteSpace(b.Isbn))
                        ?? Enumerable.Empty<ExternalBookDto>();

            if (preferQuality)
            {
                var quality = books
                    .Where(b => !string.IsNullOrWhiteSpace(b.CoverUrl)
                                && !string.Equals(b.Author, "Unknown Author", StringComparison.Ordinal))
                    .Take(take)
                    .ToList();

                if (quality.Count >= take)
                {
                    return quality;
                }

                var remaining = books.Where(b => quality.All(q => q.Isbn != b.Isbn)).Take(take - quality.Count);
                return quality.Concat(remaining);
            }

            return books.Take(take);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching books from Open Library: {Url}", url);
            return Enumerable.Empty<ExternalBookDto>();
        }
    }

    private static ExternalBookDto MapToDto(OpenLibraryDoc doc)
    {
        var isbn = doc.Isbn?.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i))
                   ?? doc.EditionKey?.FirstOrDefault(k => !string.IsNullOrWhiteSpace(k));

        return new ExternalBookDto
        {
            Title = doc.Title ?? "Unknown Title",
            Author = doc.AuthorName?.FirstOrDefault() ?? "Unknown Author",
            CoverUrl = doc.CoverI != null ? $"https://covers.openlibrary.org/b/id/{doc.CoverI}-M.jpg" : null,
            PublishedDate = doc.FirstPublishYear.HasValue ? new DateTime(doc.FirstPublishYear.Value, 1, 1) : null,
            Isbn = isbn,
            IsAvailable = doc.Availability?.IsAvailable,
            Availability = doc.Availability?.Status
        };
    }
}

internal class OpenLibrarySearchResponse
{
    [JsonPropertyName("docs")]
    public List<OpenLibraryDoc>? Docs { get; set; }
}

internal class OpenLibraryDoc
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("author_name")]
    public List<string>? AuthorName { get; set; }

    [JsonPropertyName("cover_i")]
    public int? CoverI { get; set; }

    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishYear { get; set; }

    [JsonPropertyName("isbn")]
    public List<string>? Isbn { get; set; }

    [JsonPropertyName("edition_key")]
    public List<string>? EditionKey { get; set; }

    [JsonPropertyName("availability")]
    public OpenLibraryAvailability? Availability { get; set; }
}

internal class OpenLibraryAvailability
{
    [JsonPropertyName("available_to_borrow")]
    public bool? IsAvailable { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
