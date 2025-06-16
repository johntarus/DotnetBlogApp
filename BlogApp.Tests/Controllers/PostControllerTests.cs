using System.Security.Claims;
using BlogApp.API.Controllers;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogApp.Tests.Controllers;

public class PostControllerTests
{
    private readonly Mock<IPostService> _mockPostService = new();
    private readonly Mock<ILogger<PostController>> _mockLogger = new();
    private readonly PostController _controller;

    public PostControllerTests()
    {
        _controller = new PostController(_mockPostService.Object, _mockLogger.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "User")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetAllPosts_InvalidPaging_ReturnsBadRequest()
    {
        var result = await _controller.GetAllPosts(new PostPagedRequest { PageNumber = 0, PageSize = -1 });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Page Number and Page Size must be greater than zero", badRequest.Value);
    }

    [Fact]
    public async Task GetAllPosts_ValidRequest_ReturnsOk()
    {
        var pagedPosts = new PaginatedList<PostResponseDto>(new List<PostResponseDto>(), 0, 10, 1, 0);
        _mockPostService.Setup(s => s.GetPostsAsync(It.IsAny<Guid>(), false, It.IsAny<PostPagedRequest>()))
            .ReturnsAsync(pagedPosts);

        var result = await _controller.GetAllPosts(new PostPagedRequest { PageNumber = 1, PageSize = 10 });

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(pagedPosts, okResult.Value);
    }

    [Fact]
    public async Task CreateBlog_InvalidModel_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Title", "Required");

        var result = await _controller.CreateBlog(new AddPostDto
        {
            Title = "New Post"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateBlog_ValidRequest_ReturnsCreatedAt()
    {
        var postDto = new AddPostDto { Title = "Test", Content = "Hello" };
        var response = new PostResponseDto { Id = Guid.NewGuid(), Title = postDto.Title };

        _mockPostService.Setup(s => s.CreatePostAsync(postDto, It.IsAny<Guid>())).ReturnsAsync(response);

        var result = await _controller.CreateBlog(postDto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(PostController.GetPostById), created.ActionName);
        Assert.Equal(response, created.Value);
    }

    [Fact]
    public async Task GetPostById_NotFound_ReturnsNotFound()
    {
        _mockPostService.Setup(s => s.GetPostByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((PostResponseDto)null!);

        var result = await _controller.GetPostById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetPostById_Found_ReturnsOk()
    {
        var post = new PostResponseDto { Id = Guid.NewGuid(), Title = "Test" };
        _mockPostService.Setup(s => s.GetPostByIdAsync(post.Id, It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(post);

        var result = await _controller.GetPostById(post.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(post, ok.Value);
    }

    [Fact]
    public async Task UpdatePost_InvalidModel_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Title", "Required");

        var result = await _controller.UpdatePost(Guid.NewGuid(), new UpdatePostDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdatePost_ValidRequest_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var dto = new UpdatePostDto { Title = "Updated" };
        var updatedPost = new PostResponseDto { Id = id, Title = dto.Title };

        _mockPostService.Setup(s => s.UpdatePostAsync(id, dto, It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(updatedPost);

        var result = await _controller.UpdatePost(id, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedPost, ok.Value);
    }

    [Fact]
    public async Task DeleteBlog_NotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _mockPostService.Setup(s => s.DeletePostAsync(id, It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _controller.DeleteBlog(id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteBlog_Success_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _mockPostService.Setup(s => s.DeletePostAsync(id, It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _controller.DeleteBlog(id);

        Assert.IsType<NoContentResult>(result);
    }
}
