using System.Text;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using BlogApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Tests.Repositories;

public class CommentRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly CommentRepository _repository;

    public CommentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new CommentRepository(_context);
    }

    private async Task SeedAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = Encoding.UTF8.GetBytes("secret"),
            Email = "testuser@gmail.com"
        };

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = "Test Post",
            Slug = "test-post",
            Content = "Some content",
            UserId = user.Id
        };

        var comments = new[]
        {
            new Comment { Content = "Hello World", IsEdited = false, PostId = post.Id, UserId = user.Id },
            new Comment { Content = "Another comment", IsEdited = true, PostId = post.Id, UserId = user.Id },
            new Comment { Content = "filter this", IsEdited = false, PostId = post.Id, UserId = user.Id }
        };

        await _context.Users.AddAsync(user);
        await _context.Posts.AddAsync(post);
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetCommentsAsync_FiltersBySearchQuery()
    {
        await SeedAsync();

        var result = await _repository.GetCommentsAsync(new CommentPagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            SearchQuery = "filter"
        });

        Assert.Single(result.Items);
        Assert.Contains("filter", result.Items.First().Content);
    }

    [Fact]
    public async Task GetCommentsAsync_FiltersByIsEdited()
    {
        await SeedAsync();

        var result = await _repository.GetCommentsAsync(new CommentPagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            IsEdited = true
        });

        Assert.Single(result.Items);
        Assert.True(result.Items.First().IsEdited);
    }

    [Fact]
    public async Task GetCommentsAsync_FiltersByUserId()
    {
        await SeedAsync();
        var userId = _context.Users.First().Id;

        var result = await _repository.GetCommentsAsync(new CommentPagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            UserId = userId
        });

        Assert.Equal(3, result.Items.Count);
        Assert.All(result.Items, c => Assert.Equal(userId, c.UserId));
    }

    [Fact]
    public async Task GetCommentsAsync_FiltersByPostId()
    {
        await SeedAsync();
        var postId = _context.Posts.First().Id;

        var result = await _repository.GetCommentsAsync(new CommentPagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            PostId = postId
        });

        Assert.Equal(3, result.Items.Count);
        Assert.All(result.Items, c => Assert.Equal(postId, c.PostId));
    }
}
