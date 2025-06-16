using System.Text;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using BlogApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Tests.Repositories;

public class CategoryRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new CategoryRepository(_context);
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

        var categories = new[]
        {
            new Category { Name = "Tech" },
            new Category { Name = "filterme" },
            new Category { Name = "Life" }
        };

        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = "Test Post",
            Slug = "test-post",
            Content = "Some content",
            UserId = user.Id,
            Category = categories[1]
        };

        await _context.Users.AddAsync(user);
        await _context.Categories.AddRangeAsync(categories);
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetCategoriesAsync_FiltersBySearchQuery()
    {
        await SeedAsync();

        var result = await _repository.GetCategoriesAsync(new CategoryPagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            SearchQuery = "filter"
        });

        Assert.Single(result.Items);
        Assert.Contains("filter", result.Items.First().Name, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetCategoriesAsync_FiltersById()
    {
        await SeedAsync();
        var targetId = _context.Categories.First(c => c.Name == "Tech").Id;

        var result = await _repository.GetCategoriesAsync(new CategoryPagedRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Id = targetId
        });

        Assert.Single(result.Items);
        Assert.Equal("Tech", result.Items.First().Name);
    }
}
