using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using BlogApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BlogApp.Tests.Repositories
{
    public class LikeRepositoryTests
    {
        private readonly DatabaseContext _context;
        private readonly LikeRepository _repository;

        public LikeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // use fresh DB per test
                .Options;

            _context = new DatabaseContext(options);
            _repository = new LikeRepository(_context);
        }

        private async Task SeedDataAsync()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                PasswordHash = Encoding.UTF8.GetBytes("test-password"),
                Email = "testuser@gmail.com"
            };

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = "Test Post",
                Slug = "test-post",
                Content = "Sample",
                UserId = user.Id
            };

            var like = new Like
            {
                Id = 1,
                UserId = user.Id,
                PostId = post.Id,
                CreateAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.Posts.AddAsync(post);
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetLikeByIdAsync_ReturnsLike_WhenFound()
        {
            await SeedDataAsync();
            var like = await _repository.GetLikeByIdAsync(1);
            Assert.NotNull(like);
            Assert.Equal(1, like.Id);
        }

        [Fact]
        public async Task GetLikeAsync_ReturnsCorrectLike()
        {
            await SeedDataAsync();
            var userId = _context.Users.First().Id;
            var postId = _context.Posts.First().Id;

            var like = await _repository.GetLikeAsync(postId, userId);

            Assert.NotNull(like);
            Assert.Equal(userId, like.UserId);
            Assert.Equal(postId, like.PostId);
        }

        [Fact]
        public async Task HasUserLikedPostAsync_ReturnsTrue_WhenLiked()
        {
            await SeedDataAsync();
            var userId = _context.Users.First().Id;
            var postId = _context.Posts.First().Id;

            var result = await _repository.HasUserLikedPostAsync(postId, userId);
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveLikeAsync_RemovesLikeAndReturnsTrue()
        {
            await SeedDataAsync();
            var userId = _context.Users.First().Id;
            var postId = _context.Posts.First().Id;

            var result = await _repository.RemoveLikeAsync(postId, userId);

            Assert.True(result);
            var likeStillExists = await _repository.HasUserLikedPostAsync(postId, userId);
            Assert.False(likeStillExists);
        }

        [Fact]
        public async Task AddLikeAsync_AddsLikeSuccessfully()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "newuser",
                PasswordHash = Encoding.UTF8.GetBytes("pw"),
                Email = "newuser@gmail.com"
            };
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = "Another Post",
                Slug = "another-post",
                Content = "Test",
                UserId = user.Id
            };
            await _context.Users.AddAsync(user);
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var like = new Like
            {
                UserId = user.Id,
                PostId = post.Id,
                CreateAt = DateTime.UtcNow
            };

            var added = await _repository.AddLikeAsync(like);

            Assert.NotNull(added);
            Assert.Equal(user.Id, added.UserId);
        }

        [Fact]
        public async Task GetLikesAsync_WithSearchQuery_FiltersCorrectly()
        {
            await SeedDataAsync();

            var request = new LikesPagedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                SearchQuery = "testuser"
            };

            var result = await _repository.GetLikesAsync(request);

            Assert.NotNull(result);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetLikesAsync_WithUserId_FiltersCorrectly()
        {
            await SeedDataAsync();

            var userId = _context.Users.First().Id;
            var request = new LikesPagedRequest
            {
                PageNumber = 1,
                PageSize = 10,
                UserId = userId
            };

            var result = await _repository.GetLikesAsync(request);

            Assert.NotNull(result);
            Assert.Single(result.Items);
        }
    }
}
