using AutoMapper;
using BlogApp.Core.Common.Exceptions;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Services;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogApp.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _postRepoMock = new();
    private readonly Mock<ITagRepository> _tagRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<PostService>> _loggerMock = new();

    private readonly PostService _service;

    public PostServiceTests()
    {
        _service = new PostService(_postRepoMock.Object, _tagRepoMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetPostsAsync_AsAdmin_ReturnsPosts()
    {
        var request = new PostPagedRequest { PageNumber = 1, PageSize = 10 };
        var posts = new PaginatedList<Post>(new List<Post>(), 1, 10, 0, 0);
        _postRepoMock.Setup(r => r.GetPostsAsync(request)).ReturnsAsync(posts);
        _mapperMock.Setup(m => m.Map<List<PostResponseDto>>(It.IsAny<List<Post>>()))
            .Returns(new List<PostResponseDto>());

        var result = await _service.GetPostsAsync(Guid.NewGuid(), true, request);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetPostByIdAsync_AsOwner_ReturnsPost()
    {
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var post = new Post
        {
            Id = postId,
            UserId = userId,
            Title = null
        };
        _postRepoMock.Setup(r => r.GetPostByIdAsync(postId)).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map<PostResponseDto>(post)).Returns(new PostResponseDto
        {
            Id = postId,
            Title = null
        });

        var result = await _service.GetPostByIdAsync(postId, userId, "User");

        Assert.Equal(postId, result.Id);
    }

    [Fact]
    public async Task GetPostByIdAsync_AsOtherUser_ThrowsForbidden()
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Title = null
        };
        _postRepoMock.Setup(r => r.GetPostByIdAsync(post.Id)).ReturnsAsync(post);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            _service.GetPostByIdAsync(post.Id, Guid.NewGuid(), "User"));
    }

    [Fact]
    public async Task CreatePostAsync_ValidInput_ReturnsPost()
    {
        var dto = new AddPostDto { Title = "Test", TagIds = new List<int> { 1 } };
        var userId = Guid.NewGuid();
        var post = new Post { Title = "Test", UserId = userId };
        var tags = new List<Tag> { new() { Id = 1, Name = "tag" } };
        var createdPost = new Post
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = null
        };

        _mapperMock.Setup(m => m.Map<Post>(dto)).Returns(post);
        _tagRepoMock.Setup(t => t.GetTagsByIds(dto.TagIds)).ReturnsAsync(tags);
        _postRepoMock.Setup(p => p.CreatePostAsync(It.IsAny<Post>())).ReturnsAsync(createdPost);
        _mapperMock.Setup(m => m.Map<PostResponseDto>(createdPost)).Returns(new PostResponseDto
        {
            Id = createdPost.Id,
            Title = null
        });

        var result = await _service.CreatePostAsync(dto, userId);

        Assert.NotNull(result);
        Assert.Equal(createdPost.Id, result.Id);
    }

    [Fact]
    public async Task UpdatePostAsync_AsOwner_UpdatesPost()
    {
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var updateDto = new UpdatePostDto { Title = "Updated" };
        var post = new Post
        {
            Id = postId,
            UserId = userId,
            Title = null
        };

        _postRepoMock.Setup(p => p.GetPostByIdAsync(postId)).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map(updateDto, post));
        _postRepoMock.Setup(p => p.UpdatePostAsync(post)).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map<PostResponseDto>(post)).Returns(new PostResponseDto
        {
            Id = postId,
            Title = null
        });

        var result = await _service.UpdatePostAsync(postId, updateDto, userId, "User");

        Assert.Equal(postId, result.Id);
    }

    [Fact]
    public async Task DeletePostAsync_AsAdmin_DeletesPost()
    {
        var postId = Guid.NewGuid();
        var post = new Post
        {
            Id = postId,
            UserId = Guid.NewGuid(),
            Title = null
        };

        _postRepoMock.Setup(p => p.GetPostByIdAsync(postId)).ReturnsAsync(post);
        _postRepoMock.Setup(p => p.DeletePostAsync(post)).Returns(Task.CompletedTask);

        var result = await _service.DeletePostAsync(postId, Guid.NewGuid(), "Admin");

        Assert.True(result);
    }

    [Fact]
    public async Task DeletePostAsync_NotAuthorized_ThrowsForbidden()
    {
        var postId = Guid.NewGuid();
        var post = new Post
        {
            Id = postId,
            UserId = Guid.NewGuid(),
            Title = null
        };

        _postRepoMock.Setup(p => p.GetPostByIdAsync(postId)).ReturnsAsync(post);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            _service.DeletePostAsync(postId, Guid.NewGuid(), "User"));
    }
}