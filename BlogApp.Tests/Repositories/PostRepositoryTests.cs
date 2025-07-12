using System.Text;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using BlogApp.Infrastructure.Repositories;
using BlogApp.Core.Dtos.PagedFilters;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BlogApp.Tests.Repositories
{
    public class PostRepositoryTests
    {
        private async Task<DatabaseContext> GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DatabaseContext(options);

            // Seed data
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "testuser@gmail.com",
                PasswordHash = Encoding.UTF8.GetBytes("hashed-password"),
                PasswordSalt = Encoding.UTF8.GetBytes("salted-password") // ✅ Fix: Add required field
            };

            var category = new Category
            {
                Id = 1,
                Name = "Tech"
            };

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = "Test Post",
                Content = "Some content",
                Slug = "test-post",
                UserId = user.Id,
                CategoryId = category.Id,
                User = user,
                Category = category
            };

            context.Users.Add(user);
            context.Categories.Add(category);
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GetPostsAsync_WithSearchQuery_FiltersCorrectly()
        {
            var context = await GetInMemoryDbContext();
            var repo = new PostRepository(context);

            var request = new PostPagedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                SearchQuery = "test"
            };

            var result = await repo.GetPostsAsync(request);

            Assert.Single(result.Items);
            Assert.Equal("Test Post", result.Items[0].Title);
        }

        [Fact]
        public async Task GetPostsByUserIdAsync_ReturnsCorrectPosts()
        {
            var context = await GetInMemoryDbContext();
            var repo = new PostRepository(context);

            var post = await context.Posts.FirstAsync();
            var result = await repo.GetPostsByUserIdAsync(post.UserId, 1, 10);

            Assert.Single(result.Items);
            Assert.Equal(post.Id, result.Items[0].Id);
        }

        [Fact]
        public async Task GetPostByIdAsync_ReturnsCorrectPost()
        {
            var context = await GetInMemoryDbContext();
            var repo = new PostRepository(context);

            var post = await context.Posts.FirstAsync();
            var result = await repo.GetPostByIdAsync(post.Id);

            Assert.NotNull(result);
            Assert.Equal(post.Id, result.Id);
        }
    }
}
