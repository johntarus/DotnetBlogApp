using BlogApp.Core.Utils;
using Microsoft.EntityFrameworkCore;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;

namespace BlogApp.Tests.Utils
{
    public class PaginationUtilsTests
    {
        // Use Entity Framework Core's in-memory provider for async testing
        private readonly DbContextOptions<DatabaseContext> _options;

        public PaginationUtilsTests()
        {
            _options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "PaginationTestDb")
                .Options;
        }

        [Fact]
        public async Task CreateAsync_ReturnsCorrectPagination()
        {
            // Arrange - Create real EF Core query
            using var context = new DatabaseContext(_options);
            var testData = Enumerable.Range(1, 100).Select(i => new Category { Id = i, Name = $"Category {i}" });
            await context.Categories.AddRangeAsync(testData);
            await context.SaveChangesAsync();

            var query = context.Categories.OrderBy(c => c.Id);

            // Act
            var result = await PaginationUtils.CreateAsync(query, 2, 10);

            // Assert
            Assert.Equal(100, result.TotalCount);
            Assert.Equal(10, result.TotalPages);
            Assert.Equal(10, result.Items.Count);
            Assert.Equal(11, result.Items.First().Id); // Page 2 starts at 11
        }

        [Fact]
        public async Task CreateAsync_EmptyData_ReturnsEmptyPagination()
        {
            // Arrange
            using var context = new DatabaseContext(_options);
            var query = context.Categories;

            // Act
            var result = await PaginationUtils.CreateAsync(query, 1, 10);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
    }
}