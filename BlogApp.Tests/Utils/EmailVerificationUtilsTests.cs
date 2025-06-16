using BlogApp.Core.Utils;

namespace BlogApp.Tests.Utils;

public class EmailVerificationUtilsTests
{
    [Fact]
    public void GenerateVerificationToken_ReturnsUniqueToken()
    {
        var token1 = EmailVerificationUtils.GenerateVerificationToken();
        var token2 = EmailVerificationUtils.GenerateVerificationToken();

        Assert.False(string.IsNullOrWhiteSpace(token1));
        Assert.False(string.IsNullOrWhiteSpace(token2));
        Assert.NotEqual(token1, token2);
        Assert.Matches(@"^[a-f0-9]+$", token1); // Alphanumeric hex pattern
    }

    [Fact]
    public void GenerateVerificationLink_ReturnsCorrectlyFormattedUrl()
    {
        var baseUrl = "https://example.com";
        var token = "mytoken";
        var email = "test@example.com";

        var expected = "https://example.com/verify-email?token=mytoken&email=test%40example.com";
        var result = EmailVerificationUtils.GenerateVerificationLink(baseUrl, token, email);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetTokenExpiration_Default_ReturnsFutureDate()
    {
        var expiration = EmailVerificationUtils.GetTokenExpiration();

        Assert.True(expiration > DateTime.UtcNow);
        Assert.InRange((expiration - DateTime.UtcNow).TotalHours, 23.9, 24.1); // slight margin
    }

    [Fact]
    public void GetTokenExpiration_CustomHours_ReturnsCorrectOffset()
    {
        var hours = 5;
        var expiration = EmailVerificationUtils.GetTokenExpiration(hours);

        Assert.InRange((expiration - DateTime.UtcNow).TotalHours, 4.9, 5.1); // slight margin
    }
}