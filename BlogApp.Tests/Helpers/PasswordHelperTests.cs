using BlogApp.Core.Common.Helpers;
using FluentAssertions;

namespace BlogApp.Tests.Helpers;

public class PasswordHelperTests
{
    [Fact]
    public void HashPassword_ShouldReturnDifferentHashes_ForSameInput()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var (hash1, salt1) = PasswordHelper.HashPassword(password);
        var (hash2, salt2) = PasswordHelper.HashPassword(password);

        // Assert
        hash1.Should().NotEqual(hash2); // Different salt = different hash
        salt1.Should().NotEqual(salt2); // Randomly generated
        hash1.Length.Should().Be(32);   // SHA256 hash length
        hash2.Length.Should().Be(32);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var (hash, salt) = PasswordHelper.HashPassword(password);

        // Act
        var result = PasswordHelper.VerifyPassword(password, hash, salt);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenPasswordDoesNotMatchHash()
    {
        // Arrange
        var originalPassword = "OriginalPassword!";
        var wrongPassword = "WrongPassword!";
        var (hash, salt) = PasswordHelper.HashPassword(originalPassword);

        // Act
        var result = PasswordHelper.VerifyPassword(wrongPassword, hash, salt);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_ShouldReturnCorrectLength_ForAnyInput()
    {
        // Arrange
        var password1 = "abc";
        var password2 = "a much longer password input just to test hash size consistency";

        // Act
        var (hash1, _) = PasswordHelper.HashPassword(password1);
        var (hash2, _) = PasswordHelper.HashPassword(password2);

        // Assert
        hash1.Length.Should().Be(32); // SHA256 always produces 32-byte hashes
        hash2.Length.Should().Be(32);
    }
}
