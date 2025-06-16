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

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _tagRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<TagService>> _loggerMock = new();

    private readonly TagService _service;

    public TagServiceTests()
    {
        _service = new TagService(_tagRepoMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllTags_ReturnsPaginatedTags()
    {
        var request = new TagPagedRequest { PageNumber = 1, PageSize = 10 };
        var paginatedTags = new PaginatedList<Tag>(
            new List<Tag> { new() { Id = 1, Name = "C#" } },
            request.PageNumber, request.PageSize, 1, 1);

        _tagRepoMock.Setup(r => r.GetAllTags(request)).ReturnsAsync(paginatedTags);
        _mapperMock.Setup(m => m.Map<List<TagResponseDto>>(paginatedTags.Items))
            .Returns(new List<TagResponseDto> { new() { Id = 1, Name = "C#" } });

        var result = await _service.GetAllTags(request);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("C#", result.Items[0].Name);
    }

    [Fact]
    public async Task GetTagById_TagExists_ReturnsTag()
    {
        var tag = new Tag { Id = 1, Name = "C#" };
        _tagRepoMock.Setup(r => r.GetTagById(1)).ReturnsAsync(tag);
        _mapperMock.Setup(m => m.Map<TagResponseDto>(tag)).Returns(new TagResponseDto { Id = 1, Name = "C#" });

        var result = await _service.GetTagById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("C#", result.Name);
    }

    [Fact]
    public async Task GetTagById_TagDoesNotExist_ReturnsNull()
    {
        _tagRepoMock.Setup(r => r.GetTagById(1)).ReturnsAsync((Tag)null);

        var result = await _service.GetTagById(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddTag_ValidInput_ReturnsCreatedTag()
    {
        var addDto = new AddTagDto { Name = "C#" };
        var tag = new Tag { Id = 1, Name = "C#" };
        var savedTag = new Tag { Id = 1, Name = "C#" };

        _mapperMock.Setup(m => m.Map<Tag>(addDto)).Returns(tag);
        _tagRepoMock.Setup(r => r.AddTag(tag)).ReturnsAsync(savedTag);
        _mapperMock.Setup(m => m.Map<TagResponseDto>(savedTag)).Returns(new TagResponseDto { Id = 1, Name = "C#" });

        var result = await _service.AddTag(addDto);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("C#", result.Name);
    }

    [Fact]
    public async Task AddTag_MappingFails_ReturnsNull()
    {
        var addDto = new AddTagDto { Name = "Fail" };
        _mapperMock.Setup(m => m.Map<Tag>(addDto)).Returns((Tag)null);

        var result = await _service.AddTag(addDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTag_TagExists_ReturnsUpdatedTag()
    {
        var updateDto = new UpdateTagDto { Name = "Updated" };
        var existingTag = new Tag { Id = 1, Name = "Old" };
        var updatedTag = new Tag { Id = 1, Name = "Updated" };

        _tagRepoMock.Setup(r => r.GetTagById(1)).ReturnsAsync(existingTag);
        _tagRepoMock.Setup(r => r.UpdateTag(existingTag)).ReturnsAsync(updatedTag);
        _mapperMock.Setup(m => m.Map(updateDto, existingTag));
        _mapperMock.Setup(m => m.Map<TagResponseDto>(updatedTag)).Returns(new TagResponseDto { Id = 1, Name = "Updated" });

        var result = await _service.UpdateTag(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
    }

    [Fact]
    public async Task UpdateTag_TagNotFound_ReturnsNull()
    {
        var updateDto = new UpdateTagDto { Name = "Updated" };
        _tagRepoMock.Setup(r => r.GetTagById(1)).ReturnsAsync((Tag)null);

        var result = await _service.UpdateTag(1, updateDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTag_TagExists_ReturnsTrue()
    {
        var tag = new Tag { Id = 1, Name = "DeleteMe" };
        _tagRepoMock.Setup(r => r.GetTagById(1)).ReturnsAsync(tag);
        _tagRepoMock.Setup(r => r.DeleteTag(tag)).Returns(Task.CompletedTask);

        var result = await _service.DeleteTag(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTag_TagNotFound_ReturnsFalse()
    {
        _tagRepoMock.Setup(r => r.GetTagById(1)).ReturnsAsync((Tag)null);

        var result = await _service.DeleteTag(1);

        Assert.False(result);
    }
}