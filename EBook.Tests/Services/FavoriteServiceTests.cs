using EBook.Application.Services;
using EBook.Domain.Entities;
using EBook.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EBook.Tests.Services;

public class FavoriteServiceTests
{
    private const string ClientA = "client-a";
    private const string ClientB = "client-b";

    private readonly Mock<IRepository<Favorite>> _mockFavoriteRepository;
    private readonly Mock<IRepository<Book>> _mockBookRepository;
    private readonly FavoriteService _favoriteService;

    public FavoriteServiceTests()
    {
        _mockFavoriteRepository = new Mock<IRepository<Favorite>>();
        _mockBookRepository = new Mock<IRepository<Book>>();
        _favoriteService = new FavoriteService(_mockFavoriteRepository.Object, _mockBookRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyFavoritesForClient()
    {
        var favorites = new List<Favorite>
        {
            new() { Id = 1, ClientId = ClientA, BookId = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, ClientId = ClientB, BookId = 2, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, ClientId = ClientA, Isbn = "123", CreatedAt = DateTime.UtcNow }
        };
        _mockFavoriteRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(favorites);

        var result = (await _favoriteService.GetAllAsync(ClientA)).ToList();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(f => f.ClientId == ClientA);
    }

    [Fact]
    public async Task AddAsync_WhenBookExists_AddsFavoriteWithClientId()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var favorite = new Favorite
        {
            Id = 1,
            ClientId = ClientA,
            BookId = 1,
            CreatedAt = DateTime.UtcNow
        };

        _mockFavoriteRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favorite>());
        _mockBookRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        _mockFavoriteRepository
            .Setup(r => r.AddAsync(It.IsAny<Favorite>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(favorite);

        var result = await _favoriteService.AddAsync(ClientA, 1);

        result.Should().NotBeNull();
        result.BookId.Should().Be(1);
        _mockFavoriteRepository.Verify(
            r => r.AddAsync(It.Is<Favorite>(f => f.ClientId == ClientA && f.BookId == 1), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenBookDoesNotExist_ThrowsInvalidOperationException()
    {
        _mockFavoriteRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favorite>());
        _mockBookRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _favoriteService.AddAsync(ClientA, 1));
    }

    [Fact]
    public async Task AddAsync_WhenAlreadyFavoriteForSameClient_ReturnsExisting()
    {
        var existingFavorite = new Favorite
        {
            Id = 1,
            ClientId = ClientA,
            BookId = 1,
            CreatedAt = DateTime.UtcNow
        };
        _mockFavoriteRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favorite> { existingFavorite });

        var result = await _favoriteService.AddAsync(ClientA, 1);

        result.Should().Be(existingFavorite);
        _mockBookRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockFavoriteRepository.Verify(r => r.AddAsync(It.IsAny<Favorite>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_WhenOtherClientHasSameBook_AddsForCurrentClient()
    {
        var otherClientFavorite = new Favorite
        {
            Id = 1,
            ClientId = ClientB,
            BookId = 1,
            CreatedAt = DateTime.UtcNow
        };
        var book = new Book
        {
            Id = 1,
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var added = new Favorite { Id = 2, ClientId = ClientA, BookId = 1, CreatedAt = DateTime.UtcNow };

        _mockFavoriteRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favorite> { otherClientFavorite });
        _mockBookRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        _mockFavoriteRepository
            .Setup(r => r.AddAsync(It.IsAny<Favorite>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(added);

        var result = await _favoriteService.AddAsync(ClientA, 1);

        result.ClientId.Should().Be(ClientA);
        _mockFavoriteRepository.Verify(
            r => r.AddAsync(It.Is<Favorite>(f => f.ClientId == ClientA), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_OnlyDeletesOwnFavorite()
    {
        var favorite = new Favorite { Id = 1, ClientId = ClientA, BookId = 1, CreatedAt = DateTime.UtcNow };
        _mockFavoriteRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Favorite> { favorite });
        _mockFavoriteRepository
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _favoriteService.DeleteAsync(ClientA, 1);

        _mockFavoriteRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
