using AutoMapper;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Services;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogApp.Tests.Services;

public class LikeServiceTests
{
    private readonly Mock<ILikeRepository> _likeRepoMock;
    private readonly Mock<IAuthRepository> _authRepoMock;
    private readonly Mock<IPostRepository> _postRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<LikeService>> _loggerMock;
    private readonly LikeService _likeService;

    public LikeServiceTests()
    {
        _likeRepoMock = new Mock<ILikeRepository>();
        _authRepoMock = new Mock<IAuthRepository>();
        _postRepoMock = new Mock<IPostRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<LikeService>>();

        _likeService = new LikeService(
            _likeRepoMock.Object,
            _authRepoMock.Object,
            _postRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task CreateLikeAsync_ReturnsLikeResponseDto_WhenLikeIsCreated()
    {
        // Arrange
        var likeDto = new LikeDto { PostId = Guid.Empty, UserId = Guid.NewGuid() };
        var user = new User
        {
            Id = likeDto.UserId,
            Username = null,
            Email = null
        };
        var post = new Post
        {
            Id = likeDto.PostId,
            Title = null
        };
        var like = new Like { Id = 10, PostId = Guid.Empty, UserId = likeDto.UserId };
        var responseDto = new LikeResponseDto { Id = 10, PostId = Guid.Empty, UserId = likeDto.UserId };

        _likeRepoMock.Setup(r => r.GetLikeAsync(likeDto.PostId, likeDto.UserId)).ReturnsAsync((Like)null);
        _authRepoMock.Setup(r => r.GetUserByIdAsync(likeDto.UserId)).ReturnsAsync(user);
        _postRepoMock.Setup(r => r.GetPostByIdAsync(likeDto.PostId)).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map<Like>(likeDto)).Returns(like);
        _likeRepoMock.Setup(r => r.AddLikeAsync(like)).ReturnsAsync(like);
        _mapperMock.Setup(m => m.Map<LikeResponseDto>(like)).Returns(responseDto);

        // Act
        var result = await _likeService.CreateLikeAsync(likeDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(like.Id, result.Id);
    }

    [Fact]
    public async Task CreateLikeAsync_ReturnsNull_WhenLikeAlreadyExists()
    {
        // Arrange
        var likeDto = new LikeDto { PostId = Guid.Empty, UserId = Guid.NewGuid() };
        _likeRepoMock.Setup(r => r.GetLikeAsync(likeDto.PostId, likeDto.UserId)).ReturnsAsync(new Like());

        // Act
        var result = await _likeService.CreateLikeAsync(likeDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateLikeAsync_ThrowsKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var likeDto = new LikeDto { PostId = Guid.Empty, UserId = Guid.NewGuid() };
        _likeRepoMock.Setup(r => r.GetLikeAsync(likeDto.PostId, likeDto.UserId)).ReturnsAsync((Like)null);
        _authRepoMock.Setup(r => r.GetUserByIdAsync(likeDto.UserId))!.ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _likeService.CreateLikeAsync(likeDto));
    }

    [Fact]
    public async Task CreateLikeAsync_ThrowsKeyNotFoundException_WhenPostNotFound()
    {
        // Arrange
        var likeDto = new LikeDto { PostId = Guid.Empty, UserId = Guid.NewGuid() };
        var user = new User
        {
            Id = likeDto.UserId,
            Username = null,
            Email = null
        };
        _likeRepoMock.Setup(r => r.GetLikeAsync(likeDto.PostId, likeDto.UserId)).ReturnsAsync((Like)null);
        _authRepoMock.Setup(r => r.GetUserByIdAsync(likeDto.UserId)).ReturnsAsync(user);
        _postRepoMock.Setup(r => r.GetPostByIdAsync(likeDto.PostId)).ReturnsAsync((Post)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _likeService.CreateLikeAsync(likeDto));
    }
}