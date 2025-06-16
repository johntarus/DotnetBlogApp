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
    public class CommentControllerTests
    {
        private readonly Mock<ICommentsService> _mockCommentsService;
        private readonly Mock<ILogger<CommentController>> _mockLogger;
        private readonly CommentController _controller;

        public CommentControllerTests()
        {
            _mockCommentsService = new Mock<ICommentsService>();
            _mockLogger = new Mock<ILogger<CommentController>>();
            _controller = new CommentController(null, _mockCommentsService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetComments_InvalidPagination_ReturnsBadRequest()
        {
            // Arrange
            var invalidRequest = new CommentPagedRequest { PageNumber = 0, PageSize = -1 };

            // Act
            var result = await _controller.GetComments(invalidRequest);

            // Assert - Directly check the result type since it's ActionResult (non-generic)
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetComments_ValidRequest_ReturnsServiceResult()
        {
            // Arrange
            var request = new CommentPagedRequest { PageNumber = 1, PageSize = 10 };
            var mockPaginatedList = new PaginatedList<CommentResponseDto>(
                new List<CommentResponseDto> { new CommentResponseDto() },
                1, 10, 1, 1);

            _mockCommentsService.Setup(s => s.GetCommentsAsync(request))
                .ReturnsAsync(mockPaginatedList);

            // Act
            var result = await _controller.GetComments(request);

            // Assert - Directly check the result type since it's ActionResult (non-generic)
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mockPaginatedList, okResult.Value);
            _mockCommentsService.Verify(s => s.GetCommentsAsync(request), Times.Once);
        }

        [Fact]
        public async Task GetCommentById_NotFound_Returns404()
        {
            // Arrange
            _mockCommentsService.Setup(s => s.GetCommentsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((CommentResponseDto)null!);

            // Act
            var result = await _controller.GetCommentById(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CommentResponseDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetCommentById_Found_ReturnsOk()
        {
            // Arrange
            var expectedComment = new CommentResponseDto { Id = 1 };
            _mockCommentsService.Setup(s => s.GetCommentsByIdAsync(1))
                .ReturnsAsync(expectedComment);

            // Act
            var result = await _controller.GetCommentById(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CommentResponseDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(expectedComment, okResult.Value);
        }

        [Fact]
        public async Task CreateComment_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Content", "Required");

            // Act
            var result = await _controller.CreateComment(new CommentDto
            {
                Content = null
            });

            // Assert
            var actionResult = Assert.IsType<ActionResult<CommentResponseDto>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task CreateComment_ValidRequest_ReturnsCreatedComment()
        {
            // Arrange
            var commentDto = new CommentDto { Content = "Test" };
            var expectedResponse = new CommentResponseDto { Id = 1, Content = "Test" };
            _mockCommentsService.Setup(s => s.CreateCommentAsync(commentDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateComment(commentDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CommentResponseDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task UpdateComment_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Content", "Required");

            // Act
            var result = await _controller.UpdateComment(1, new UpdateCommentDto());

            // Assert
            var actionResult = Assert.IsType<ActionResult<CommentResponseDto>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task UpdateComment_Success_ReturnsUpdatedComment()
        {
            // Arrange
            var updateDto = new UpdateCommentDto { Content = "Updated" };
            var expectedResponse = new CommentResponseDto { Id = 1, Content = "Updated" };
            _mockCommentsService.Setup(s => s.UpdateCommentAsync(1, updateDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdateComment(1, updateDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CommentResponseDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task DeleteComment_NotExists_ReturnsNotFound()
        {
            // Arrange
            _mockCommentsService.Setup(s => s.DeleteCommentAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteComment(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteComment_Success_ReturnsNoContent()
        {
            // Arrange
            _mockCommentsService.Setup(s => s.DeleteCommentAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteComment(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}