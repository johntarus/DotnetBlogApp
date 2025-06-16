using AutoMapper;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Services;
using BlogApp.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogApp.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CategoryService>> _loggerMock = new();
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _service = new CategoryService(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetCategoriesAsync_ShouldReturnPaginatedList()
    {
        // Arrange
        var request = new CategoryPagedRequest { PageNumber = 1, PageSize = 2 };
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Tech" },
            new() { Id = 2, Name = "Life" }
        };

        var pagedResult = new PaginatedList<Category>(categories, 1, 2, 2, 1);
        var categoryDtos = new List<CategoryResponseDto>
        {
            new() { Id = 1, Name = "Tech" },
            new() { Id = 2, Name = "Life" }
        };

        _repoMock.Setup(r => r.GetCategoriesAsync(request)).ReturnsAsync(pagedResult);
        _mapperMock.Setup(m => m.Map<List<CategoryResponseDto>>(categories)).Returns(categoryDtos);

        // Act
        var result = await _service.GetCategoriesAsync(request);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.Items.Should().BeEquivalentTo(categoryDtos);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnDto_WhenFound()
    {
        var category = new Category { Id = 1, Name = "Tech" };
        var categoryDto = new CategoryResponseDto { Id = 1, Name = "Tech" };

        _repoMock.Setup(r => r.GetCategoryByIdAsync(1)).ReturnsAsync(category);
        _mapperMock.Setup(m => m.Map<CategoryResponseDto>(category)).Returns(categoryDto);

        var result = await _service.GetCategoryById(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(categoryDto);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetCategoryByIdAsync(1)).ReturnsAsync((Category)null);

        var result = await _service.GetCategoryById(1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddCategoryAsync_ShouldReturnCreatedCategory()
    {
        var dto = new AddCategoryDto { Name = "Tech" };
        var category = new Category { Id = 1, Name = "Tech" };
        var categoryResponse = new CategoryResponseDto { Id = 1, Name = "Tech" };

        _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(category);
        _repoMock.Setup(r => r.CreateCategoryAsync(category)).ReturnsAsync(category);
        _mapperMock.Setup(m => m.Map<CategoryResponseDto>(category)).Returns(categoryResponse);

        var result = await _service.AddCategoryAsync(dto);

        result.Should().BeEquivalentTo(categoryResponse);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldUpdateAndReturnCategory_WhenExists()
    {
        var dto = new UpdateCategoryDto { Name = "Updated" };
        var category = new Category { Id = 1, Name = "Old" };
        var updatedCategory = new Category { Id = 1, Name = "Updated" };
        var updatedDto = new CategoryResponseDto { Id = 1, Name = "Updated" };

        _repoMock.Setup(r => r.GetCategoryByIdAsync(1)).ReturnsAsync(category);
        _mapperMock.Setup(m => m.Map(dto, category));
        _repoMock.Setup(r => r.UpdateCategoryAsync(category)).ReturnsAsync(updatedCategory);
        _mapperMock.Setup(m => m.Map<CategoryResponseDto>(updatedCategory)).Returns(updatedDto);

        var result = await _service.UpdateCategoryAsync(1, dto);

        result.Should().BeEquivalentTo(updatedDto);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldReturnNull_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetCategoryByIdAsync(1)).ReturnsAsync((Category)null);

        var result = await _service.UpdateCategoryAsync(1, new UpdateCategoryDto { Name = "Updated" });

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldReturnTrue_WhenDeleted()
    {
        var category = new Category { Id = 1, Name = "ToDelete" };

        _repoMock.Setup(r => r.GetCategoryByIdAsync(1)).ReturnsAsync(category);
        _repoMock.Setup(r => r.DeleteCategoryAsync(category)).Returns(Task.CompletedTask);

        var result = await _service.DeleteCategoryAsync(1);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldReturnFalse_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetCategoryByIdAsync(1)).ReturnsAsync((Category)null);

        var result = await _service.DeleteCategoryAsync(1);

        result.Should().BeFalse();
    }
}
