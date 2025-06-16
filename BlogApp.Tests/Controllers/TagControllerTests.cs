using System.Security.Claims;
using BlogApp.API.Controllers;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using BlogApp.Domain.Entities;

namespace BlogApp.Tests.Controllers
{
    public class TagControllerTests
    {
        private readonly Mock<ITagService> _mockTagService;
        private readonly Mock<ILogger<TagController>> _mockLogger;
        private readonly TagController _controller;

        public TagControllerTests()
        {
            _mockTagService = new Mock<ITagService>();
            _mockLogger = new Mock<ILogger<TagController>>();
            
            // Setup admin user
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "TestAuthentication"));
            
            _controller = new TagController(_mockTagService.Object, _mockLogger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = adminUser }
                }
            };
        }

        [Fact]
        public async Task GetTags_InvalidPagination_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequest = new TagPagedRequest { PageNumber = 0, PageSize = -1 };

            // Act
            var result = await _controller.GetTags(invalidRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetTags_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new TagPagedRequest { PageNumber = 1, PageSize = 10 };
            var mockTags = new PaginatedList<TagResponseDto>(
                new List<TagResponseDto> { new TagResponseDto() }, 
                1, 10, 1, 1);
            
            _mockTagService.Setup(s => s.GetAllTags(request))
                .ReturnsAsync(mockTags);

            // Act
            var result = await _controller.GetTags(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mockTags, okResult.Value);
        }

        [Fact]
        public async Task CreateTag_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreateTag(new AddTagDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateTag_ValidRequest_ReturnsOk()
        {
            // Arrange
            var tagDto = new AddTagDto { Name = "Test" };
            var expectedTag = new TagResponseDto { Id = 1, Name = "Test" };
            _mockTagService.Setup(s => s.AddTag(tagDto))
                .ReturnsAsync(expectedTag);

            // Act
            var result = await _controller.CreateTag(tagDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedTag, okResult.Value);
        }

        [Fact]
        public async Task UpdateTag_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.UpdateTag(1, new UpdateTagDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateTag_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockTagService.Setup(s => s.UpdateTag(It.IsAny<int>(), It.IsAny<UpdateTagDto>()))
                .ReturnsAsync((TagResponseDto)null);

            // Act
            var result = await _controller.UpdateTag(1, new UpdateTagDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateTag_Success_ReturnsOk()
        {
            // Arrange
            var updateDto = new UpdateTagDto { Name = "Updated" };
            var expectedTag = new TagResponseDto { Id = 1, Name = "Updated" };
            _mockTagService.Setup(s => s.UpdateTag(1, updateDto))
                .ReturnsAsync(expectedTag);

            // Act
            var result = await _controller.UpdateTag(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedTag, okResult.Value);
        }

        [Fact]
        public async Task GetTagById_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockTagService.Setup(s => s.GetTagById(It.IsAny<int>()))
                .ReturnsAsync((TagResponseDto)null);

            // Act
            var result = await _controller.GetTagById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTagById_Found_ReturnsOk()
        {
            // Arrange
            var expectedTag = new TagResponseDto { Id = 1, Name = "Test" };
            _mockTagService.Setup(s => s.GetTagById(1))
                .ReturnsAsync(expectedTag);

            // Act
            var result = await _controller.GetTagById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedTag, okResult.Value);
        }

        [Fact]
        public async Task DeleteTag_NotFound_ReturnsNotFound()
        {
            // Arrange
            _mockTagService.Setup(s => s.DeleteTag(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTag(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTag_Success_ReturnsNoContent()
        {
            // Arrange
            _mockTagService.Setup(s => s.DeleteTag(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTag(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}