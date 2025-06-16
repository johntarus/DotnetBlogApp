using BlogApp.Core.Utils;
using Xunit;

namespace BlogApp.Tests.Utils;

public class SlugUtilsTests
{
    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("   Leading and trailing spaces   ", "leading-and-trailing-spaces")]
    [InlineData("Special!@#$%^&*()Characters", "specialcharacters")]
    [InlineData("Multiple     Spaces", "multiple-spaces")]
    [InlineData("Already-slugified-text", "already-slugified-text")]
    [InlineData("Title With UPPERCASE Letters", "title-with-uppercase-letters")]
    [InlineData("Text-With---Multiple---Hyphens", "text-with-multiple-hyphens")]
    [InlineData("C# .NET 8 Preview", "c-net-8-preview")]
    [InlineData("", "")] // Empty string
    [InlineData("123 Numbers First", "123-numbers-first")]
    public void GenerateSlug_ReturnsExpectedSlug(string input, string expected)
    {
        var slug = SlugUtils.GenerateSlug(input);
        Assert.Equal(expected, slug);
    }
}