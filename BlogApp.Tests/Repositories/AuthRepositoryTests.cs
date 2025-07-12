using BlogApp.Core.Common.Helpers;
using BlogApp.Domain.Entities;
using BlogApp.Infrastructure.Data;
using BlogApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Tests.Repositories;

public class AuthRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly AuthRepository _repository;

    public AuthRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new AuthRepository(_context);
    }

    private async Task SeedAsync()
    {
        var (hash, salt) = PasswordHelper.HashPassword("secret");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "TestUser",
            Email = "test@example.com",
            PasswordHash = hash,
            PasswordSalt = salt
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenEmailMatches_CaseInsensitive()
    {
        await SeedAsync();

        var result = await _repository.GetByEmailAsync("TEST@EXAMPLE.COM");

        Assert.NotNull(result);
        Assert.Equal("TestUser", result!.Username);
    }

    [Fact]
    public async Task GetByUsernameOrEmailAsync_ReturnsUser_ByUsername()
    {
        await SeedAsync();

        var result = await _repository.GetByUsernameOrEmailAsync("TestUser");

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result!.Email);
    }

    [Fact]
    public async Task GetByUsernameOrEmailAsync_ReturnsUser_ByEmail()
    {
        await SeedAsync();

        var result = await _repository.GetByUsernameOrEmailAsync("test@example.com");

        Assert.NotNull(result);
        Assert.Equal("TestUser", result!.Username);
    }

    [Fact]
    public async Task ExistsByEmailAsync_ReturnsTrue_IfEmailExists()
    {
        await SeedAsync();

        var exists = await _repository.ExistsByEmailAsync("test@example.com");

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByEmailAsync_ReturnsFalse_IfEmailDoesNotExist()
    {
        await SeedAsync();

        var exists = await _repository.ExistsByEmailAsync("missing@example.com");

        Assert.False(exists);
    }
}
