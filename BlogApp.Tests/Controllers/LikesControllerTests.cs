using BlogApp.API.Controllers;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogApp.Tests.Controllers;

public class LikesControllerTests
{
    private readonly Mock<ILikeService> _mockLikeService;
    private readonly Mock<ILogger<LikesController>> _mockLogger;
    private readonly LikesController _controller;

    public LikesControllerTests()
    {
        _mockLikeService = new Mock<ILikeService>();
        _mockLogger = new Mock<ILogger<LikesController>>();
        _controller = new LikesController(_mockLikeService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetLikes_InvalidPagination_ReturnsBadRequest()
    {
        var request = new LikesPagedRequest { PageNumber = 0, PageSize = -5 };
        var result = await _controller.GetLikes(request);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetLikes_ValidRequest_ReturnsOk()
    {
        var request = new LikesPagedRequest { PageNumber = 1, PageSize = 10 };
        var likes = new PaginatedList<LikeResponseDto>(
            new List<LikeResponseDto> { new LikeResponseDto() }, 1, 10, 1, 1);

        _mockLikeService.Setup(s => s.GetLikesAsync(request)).ReturnsAsync(likes);
        var result = await _controller.GetLikes(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(likes, okResult.Value);
    }

    [Fact]
    public async Task GetLikeById_NotFound_ReturnsNotFound()
    {
        _mockLikeService.Setup(s => s.GetLikeByIdAsync(It.IsAny<int>())).ReturnsAsync((LikeResponseDto)null!);
        var result = await _controller.GetLikeById(1);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetLikeById_Found_ReturnsOk()
    {
        var like = new LikeResponseDto { Id = 1 };
        _mockLikeService.Setup(s => s.GetLikeByIdAsync(1)).ReturnsAsync(like);
        var result = await _controller.GetLikeById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(like, okResult.Value);
    }

    [Fact]
    public async Task AddLike_Success_ReturnsOk()
    {
        var dto = new LikeDto { UserId = Guid.Empty, PostId = Guid.Empty };
        var response = new LikeResponseDto { Id = 1 };

        _mockLikeService.Setup(s => s.CreateLikeAsync(dto)).ReturnsAsync(response);
        var result = await _controller.AddLike(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task AddLike_Conflict_ReturnsConflict()
    {
        var dto = new LikeDto { UserId = Guid.Empty, PostId = Guid.Empty };
        _mockLikeService.Setup(s => s.CreateLikeAsync(dto)).ReturnsAsync((LikeResponseDto)null!);

        var result = await _controller.AddLike(dto);
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("Already liked", conflictResult.Value);
    }

    [Fact]
    public async Task RemoveLike_NotFound_ReturnsNotFound()
    {
        var dto = new LikeDto { UserId = Guid.Empty, PostId = Guid.Empty };
        _mockLikeService.Setup(s => s.RemoveLikeAsync(dto)).ReturnsAsync(false);

        var result = await _controller.RemoveLike(dto);
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Like not found", notFound.Value);
    }

    [Fact]
    public async Task RemoveLike_Success_ReturnsOk()
    {
        var dto = new LikeDto { UserId = Guid.Empty, PostId = Guid.Empty };
        _mockLikeService.Setup(s => s.RemoveLikeAsync(dto)).ReturnsAsync(true);

        var result = await _controller.RemoveLike(dto);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Post unliked", okResult.Value);
    }

    [Fact]
    public async Task CheckIfUserLikedPost_ReturnsOkWithResult()
    {
        var dto = new CheckLikeRequestDto { UserId = Guid.Empty, PostId = Guid.Empty };
        _mockLikeService.Setup(s => s.CheckIfLikedAsync(dto)).ReturnsAsync(true);

        var result = await _controller.CheckIfUserLikedPost(dto);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!.GetType().GetProperty("hasLiked")!.GetValue(okResult.Value)!);
    }
}
