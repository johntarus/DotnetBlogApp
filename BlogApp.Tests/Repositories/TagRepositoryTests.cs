using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using BlogApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BlogApp.Tests.Repositories;

public class TagRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new TagRepository(_context);
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

        var tags = new[]
        {
            new Tag { Name = "Tech" },
            new Tag { Name = "filterme" },
            new Tag { Name = "Life" }
        };

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = "Tagged Post",
            Slug = "tagged-post",
            Content = "Some content",
            UserId = user.Id,
            Tags = new List<Tag> { tags[1] }
        };

        await _context.Users.AddAsync(user);
        await _context.Tags.AddRangeAsync(tags);
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllTags_FiltersBySearchQuery()
    {
        await SeedAsync();

        var result = await _repository.GetAllTags(new TagPagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            SearchQuery = "filter"
        });

        Assert.Single(result.Items);
        Assert.Contains("filter", result.Items.First().Name, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAllTags_FiltersById()
    {
        await SeedAsync();
        var tagId = _context.Tags.First(t => t.Name == "Tech").Id;

        var result = await _repository.GetAllTags(new TagPagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Id = tagId
        });

        Assert.Single(result.Items);
        Assert.Equal("Tech", result.Items.First().Name);
    }
}
