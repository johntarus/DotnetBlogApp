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

public class TagControllerTests
{
    private readonly Mock<ITagService> _mockTagService = new();
    private readonly Mock<ILogger<TagController>> _mockLogger = new();
    private readonly TagController _controller;

    public TagControllerTests()
    {
        _controller = new TagController(_mockTagService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetTags_InvalidPaging_ReturnsBadRequest()
    {
        var request = new TagPagedRequest { PageNumber = 0, PageSize = 0 };

        var result = await _controller.GetTags(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Page Number and Page Size must be greater than zero", badRequest.Value);
    }

    [Fact]
    public async Task GetTags_ValidRequest_ReturnsOk()
    {
        var request = new TagPagedRequest { PageNumber = 1, PageSize = 10 };
        var tags = new PaginatedList<TagResponseDto>(new List<TagResponseDto>(), 1, 10, 0, 0);

        _mockTagService.Setup(s => s.GetAllTags(request)).ReturnsAsync(tags);

        var result = await _controller.GetTags(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tags, ok.Value);
    }

    [Fact]
    public async Task CreateTag_ValidRequest_ReturnsOk()
    {
        var dto = new AddTagDto { Name = "Test" };
        var tag = new TagResponseDto { Id = 1, Name = "Test" };

        _mockTagService.Setup(s => s.AddTag(dto)).ReturnsAsync(tag);

        var result = await _controller.CreateTag(dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tag, ok.Value);
    }

    [Fact]
    public async Task GetTagById_NotFound_ReturnsNotFound()
    {
        _mockTagService.Setup(s => s.GetTagById(1)).ReturnsAsync((TagResponseDto)null!);

        var result = await _controller.GetTagById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTagById_Found_ReturnsOk()
    {
        var tag = new TagResponseDto { Id = 1, Name = "Test" };

        _mockTagService.Setup(s => s.GetTagById(1)).ReturnsAsync(tag);

        var result = await _controller.GetTagById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tag, ok.Value);
    }

    [Fact]
    public async Task UpdateTag_NotFound_ReturnsNotFound()
    {
        var dto = new UpdateTagDto { Name = "Updated" };

        _mockTagService.Setup(s => s.UpdateTag(1, dto)).ReturnsAsync((TagResponseDto)null!);

        var result = await _controller.UpdateTag(1, dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateTag_ValidRequest_ReturnsOk()
    {
        var dto = new UpdateTagDto { Name = "Updated" };
        var updated = new TagResponseDto { Id = 1, Name = dto.Name };

        _mockTagService.Setup(s => s.UpdateTag(1, dto)).ReturnsAsync(updated);

        var result = await _controller.UpdateTag(1, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updated, ok.Value);
    }

    [Fact]
    public async Task DeleteTag_NotFound_ReturnsNotFound()
    {
        _mockTagService.Setup(s => s.DeleteTag(1)).ReturnsAsync(false);

        var result = await _controller.DeleteTag(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteTag_Success_ReturnsNoContent()
    {
        _mockTagService.Setup(s => s.DeleteTag(1)).ReturnsAsync(true);

        var result = await _controller.DeleteTag(1);

        Assert.IsType<NoContentResult>(result);
    }
}
