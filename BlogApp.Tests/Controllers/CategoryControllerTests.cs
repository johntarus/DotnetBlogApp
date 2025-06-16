using BlogApp.API.Controllers;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogApp.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly Mock<ILogger<CategoryController>> _mockLogger;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _mockLogger = new Mock<ILogger<CategoryController>>();
            _controller = new CategoryController(_mockCategoryService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCategories_InvalidPagination_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequest = new CategoryPagedRequest { PageNumber = 0, PageSize = -1 };

            // Act
            var result = await _controller.GetCategories(invalidRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetCategories_ValidRequest_ReturnsServiceResult()
        {
            // Arrange
            var request = new CategoryPagedRequest { PageNumber = 1, PageSize = 10 };
            var mockPaginatedList = new PaginatedList<CategoryResponseDto>(
                new List<CategoryResponseDto> { new CategoryResponseDto() },
                1, 10, 1, 1);

            _mockCategoryService.Setup(s => s.GetCategoriesAsync(request))
                .ReturnsAsync(mockPaginatedList);

            // Act
            var result = await _controller.GetCategories(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockCategoryService.Verify(s => s.GetCategoriesAsync(request), Times.Once);
        }

        [Fact]
        public async Task GetCategoryById_NotFound_Returns404()
        {
            // Arrange
            _mockCategoryService.Setup(s => s.GetCategoryById(It.IsAny<int>()))
                .ReturnsAsync((CategoryResponseDto)null!);

            // Act
            var result = await _controller.GetCategoryById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateCategory_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreateCategory(new AddCategoryDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateCategory_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.UpdateCategory(1, new UpdateCategoryDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCategory_NotExists_ReturnsNotFound()
        {
            // Arrange
            _mockCategoryService.Setup(s => s.DeleteCategoryAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteCategory(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}