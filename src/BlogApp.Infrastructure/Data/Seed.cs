using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Infrastructure.Data;

public static class Seed
{
    public static void SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        // Apply any pending migrations
        context.Database.Migrate();

        // Seed only if users table is empty
        if (!context.Users.Any())
        {
            Console.WriteLine("Seeding initial data...");

            SeedUsers(context);
            SeedCategories(context);
            SeedTags(context);
            SeedPosts(context);
            SeedComments(context);
            SeedLikes(context);
        }
    }

    private static void SeedUsers(DatabaseContext context)
    {
        var password = "Password123!";

        var users = new List<User>();

        void AddUser(Guid id, string username, string email, string role)
        {
            using var hmac = new HMACSHA256();
            var user = new User
            {
                Id = id,
                Username = username,
                Email = email,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                Role = role,
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            users.Add(user);
        }

        AddUser(Guid.Parse("00000000-0000-0000-0000-000000000001"), "admin", "admin@example.com", "Admin");
        AddUser(Guid.Parse("00000000-0000-0000-0000-000000000002"), "johndoe", "john@example.com", "Author");
        AddUser(Guid.Parse("00000000-0000-0000-0000-000000000003"), "janedoe", "jane@example.com", "Reader");

        context.Users.AddRange(users);
        context.SaveChanges();
    }

    private static void SeedCategories(DatabaseContext context)
    {
        var categories = new List<Category>
        {
            new Category { Name = "Technology" },
            new Category { Name = "Travel" },
            new Category { Name = "Food" },
            new Category { Name = "Lifestyle" },
            new Category { Name = "Business" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();
    }

    private static void SeedTags(DatabaseContext context)
    {
        var now = DateTime.UtcNow;
        var tags = new List<Tag>
        {
            new Tag { Name = "C#", CreatedAt = now, UpdatedAt = now },
            new Tag { Name = "ASP.NET", CreatedAt = now, UpdatedAt = now },
            new Tag { Name = "Programming", CreatedAt = now, UpdatedAt = now },
            new Tag { Name = "Europe", CreatedAt = now, UpdatedAt = now },
            new Tag { Name = "Recipes", CreatedAt = now, UpdatedAt = now },
            new Tag { Name = "Productivity", CreatedAt = now, UpdatedAt = now }
        };

        context.Tags.AddRange(tags);
        context.SaveChanges();
    }

    private static void SeedPosts(DatabaseContext context)
    {
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var johnId = Guid.Parse("00000000-0000-0000-0000-000000000002");

        var techCategory = context.Categories.First(c => c.Name == "Technology");
        var travelCategory = context.Categories.First(c => c.Name == "Travel");
        var foodCategory = context.Categories.First(c => c.Name == "Food");

        var csharpTag = context.Tags.First(t => t.Name == "C#");
        var aspnetTag = context.Tags.First(t => t.Name == "ASP.NET");
        var programmingTag = context.Tags.First(t => t.Name == "Programming");
        var europeTag = context.Tags.First(t => t.Name == "Europe");

        var posts = new List<Post>
        {
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "Getting Started with ASP.NET Core",
                Content = "ASP.NET Core is a cross-platform, high-performance framework...",
                Slug = "getting-started-with-aspnet-core",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                UserId = adminId,
                CategoryId = techCategory.Id,
                Tags = new List<Tag> { csharpTag, aspnetTag, programmingTag }
            },
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "My Travel Adventures in Europe",
                Content = "Last summer I visited several amazing countries in Europe...",
                Slug = "travel-adventures-europe",
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                UserId = johnId,
                CategoryId = travelCategory.Id,
                Tags = new List<Tag> { europeTag }
            },
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "The Best Italian Pasta Recipes",
                Content = "Here are my favorite authentic Italian pasta recipes...",
                Slug = "best-italian-pasta-recipes",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow,
                UserId = adminId,
                CategoryId = foodCategory.Id,
                Tags = new List<Tag>()
            }
        };

        context.Posts.AddRange(posts);
        context.SaveChanges();
    }

    private static void SeedComments(DatabaseContext context)
    {
        var firstPost = context.Posts.First();
        var secondPost = context.Posts.Skip(1).First();

        var janeId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var johnId = Guid.Parse("00000000-0000-0000-0000-000000000002");

        var comments = new List<Comment>
        {
            new Comment
            {
                Content = "Great tutorial! Very helpful for beginners.",
                CreatedAt = DateTime.UtcNow.AddDays(-9),
                UpdatedAt = DateTime.UtcNow.AddDays(-9),
                PostId = firstPost.Id,
                UserId = janeId
            },
            new Comment
            {
                Content = "I had a question about middleware configuration.",
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow.AddDays(-7),
                IsEdited = true,
                PostId = firstPost.Id,
                UserId = johnId
            },
            new Comment
            {
                Content = "Which countries would you recommend visiting first?",
                CreatedAt = DateTime.UtcNow.AddDays(-6),
                UpdatedAt = DateTime.UtcNow.AddDays(-6),
                PostId = secondPost.Id,
                UserId = janeId
            }
        };

        context.Comments.AddRange(comments);
        context.SaveChanges();
    }

    private static void SeedLikes(DatabaseContext context)
    {
        var firstPost = context.Posts.First();
        var secondPost = context.Posts.Skip(1).First();

        var janeId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var johnId = Guid.Parse("00000000-0000-0000-0000-000000000002");

        var likes = new List<Like>
        {
            new Like
            {
                PostId = firstPost.Id,
                UserId = janeId,
                CreateAt = DateTime.UtcNow.AddDays(-9)
            },
            new Like
            {
                PostId = firstPost.Id,
                UserId = johnId,
                CreateAt = DateTime.UtcNow.AddDays(-8)
            },
            new Like
            {
                PostId = secondPost.Id,
                UserId = janeId,
                CreateAt = DateTime.UtcNow.AddDays(-6)
            }
        };

        context.Likes.AddRange(likes);
        context.SaveChanges();
    }
}
