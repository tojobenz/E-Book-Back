using EBook.Application.Services;
using EBook.Domain.Entities;
using EBook.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EBook.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IRepository<Book>> _mockBookRepository;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _mockBookRepository = new Mock<IRepository<Book>>();
        _bookService = new BookService(_mockBookRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBookExists_ReturnsBook()
    {
        // Arrange
        var expectedBook = new Book
        {
            Id = 1,
            Title = "Test Book",
            Author = "Test Author",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mockBookRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBook);

        // Act
        var result = await _bookService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedBook);
        _mockBookRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBookDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockBookRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockBookRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }
}
