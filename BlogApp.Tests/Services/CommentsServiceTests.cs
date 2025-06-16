using AutoMapper;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Services;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogApp.Tests.Services;

public class CommentsServiceTests
{
    private readonly Mock<ICommentRepository> _commentRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly ILogger<CommentsService> _logger = new LoggerFactory().CreateLogger<CommentsService>();
    private readonly CommentsService _service;

    public CommentsServiceTests()
    {
        _service = new CommentsService(_commentRepoMock.Object, _mapperMock.Object, _logger);
    }

    [Fact]
    public async Task GetCommentsAsync_ReturnsPaginatedList()
    {
        // Arrange
        var request = new CommentPagedRequest { PageNumber = 1, PageSize = 2 };
        var domainComments = new List<Comment> { new() { Id = 1, Content = "First Comment"}, new() { Id = 2, Content = "Second Comment"} };
        var pagedList = new PaginatedList<Comment>(domainComments, 1, 2, 2, 1);

        _commentRepoMock.Setup(r => r.GetCommentsAsync(request)).ReturnsAsync(pagedList);
        _mapperMock.Setup(m => m.Map<List<CommentResponseDto>>(domainComments))
            .Returns(new List<CommentResponseDto> { new() { Id = 1, Content = "First Comment" }, new() { Id = 2, Content = "Second Comment" } });

        // Act
        var result = await _service.GetCommentsAsync(request);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        _commentRepoMock.Verify(r => r.GetCommentsAsync(request), Times.Once);
    }

    [Fact]
    public async Task CreateCommentAsync_ReturnsCreatedComment()
    {
        var commentDto = new CommentDto { PostId = Guid.Empty, Content = "Test" };
        var domainComment = new Comment { PostId = Guid.Empty, Content = "Test" };
        var savedComment = new Comment { Id = 10, PostId = Guid.Empty, Content = "Test" };
        var responseDto = new CommentResponseDto { Id = 10, Content = "Test" };

        _mapperMock.Setup(m => m.Map<Comment>(commentDto)).Returns(domainComment);
        _commentRepoMock.Setup(r => r.CreateCommentAsync(domainComment)).ReturnsAsync(savedComment);
        _mapperMock.Setup(m => m.Map<CommentResponseDto>(savedComment)).Returns(responseDto);

        var result = await _service.CreateCommentAsync(commentDto);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Test", result.Content);
    }

    [Fact]
    public async Task GetCommentsByIdAsync_ReturnsComment_WhenExists()
    {
        var id = 1;
        var comment = new Comment { Id = id, Content = "Content" };
        var response = new CommentResponseDto { Id = id, Content = "Content" };

        _commentRepoMock.Setup(r => r.GetCommentByIdAsync(id)).ReturnsAsync(comment);
        _mapperMock.Setup(m => m.Map<CommentResponseDto>(comment)).Returns(response);

        var result = await _service.GetCommentsByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetCommentsByIdAsync_ReturnsNull_WhenNotFound()
    {
        _commentRepoMock.Setup(r => r.GetCommentByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);

        var result = await _service.GetCommentsByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCommentAsync_UpdatesAndReturnsComment_WhenExists()
    {
        var id = 5;
        var updateDto = new UpdateCommentDto { Content = "Updated" };
        var domainComment = new Comment { Id = id, Content = "Old content" };
        var updated = new Comment { Id = id, Content = "Updated", UpdatedAt = DateTime.UtcNow, IsEdited = true };
        var response = new CommentResponseDto { Id = id, Content = "Updated" };

        _commentRepoMock.Setup(r => r.GetCommentByIdAsync(id)).ReturnsAsync(domainComment);
        _mapperMock.Setup(m => m.Map(updateDto, domainComment)); // Just mapping, no return
        _commentRepoMock.Setup(r => r.UpdateCommentAsync(domainComment)).ReturnsAsync(updated);
        _mapperMock.Setup(m => m.Map<CommentResponseDto>(updated)).Returns(response);

        var result = await _service.UpdateCommentAsync(id, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result.Content);
    }

    [Fact]
    public async Task UpdateCommentAsync_ReturnsNull_WhenNotFound()
    {
        _commentRepoMock.Setup(r => r.GetCommentByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);

        var result = await _service.UpdateCommentAsync(999, new UpdateCommentDto());

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteCommentAsync_Deletes_WhenExists()
    {
        var comment = new Comment { Id = 7, Content = "Comment to Delete"};

        _commentRepoMock.Setup(r => r.GetCommentByIdAsync(7)).ReturnsAsync(comment);
        _commentRepoMock.Setup(r => r.DeleteCommentAsync(comment)).Returns(Task.CompletedTask);

        var result = await _service.DeleteCommentAsync(7);

        Assert.True(result);
        _commentRepoMock.Verify(r => r.DeleteCommentAsync(comment), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_ReturnsFalse_WhenNotFound()
    {
        _commentRepoMock.Setup(r => r.GetCommentByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);

        var result = await _service.DeleteCommentAsync(123);

        Assert.False(result);
    }
}