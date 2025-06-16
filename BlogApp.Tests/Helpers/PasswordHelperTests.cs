using BlogApp.Core.Common.Helpers;
using FluentAssertions;

namespace BlogApp.Tests.Helpers;

public class PasswordHelperTests
{
    [Fact]
    public void HashPassword_ShouldReturnConsistentHash_ForSameInput()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var hash1 = PasswordHelper.HashPassword(password);
        var hash2 = PasswordHelper.HashPassword(password);

        // Assert
        hash1.Should().Equal(hash2); // Same input -> same hash
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var hash = PasswordHelper.HashPassword(password);

        // Act
        var result = PasswordHelper.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
    {
        // Arrange
        var originalPassword = "OriginalPassword!";
        var wrongPassword = "WrongPassword!";
        var hash = PasswordHelper.HashPassword(originalPassword);

        // Act
        var result = PasswordHelper.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_ShouldReturnSameLength_ForAnyInput()
    {
        // Arrange
        var password1 = "abc";
        var password2 = "a much longer password input just to test hash size consistency";

        // Act
        var hash1 = PasswordHelper.HashPassword(password1);
        var hash2 = PasswordHelper.HashPassword(password2);

        // Assert
        hash1.Length.Should().Be(32); // SHA256 always produces 32-byte hashes
        hash2.Length.Should().Be(32);
    }
}