using System.Text.RegularExpressions;

namespace BlogApp.Helpers;

public class SlugUtils
{
    public static string GenerateSlug(string title)
    {
        string slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
        slug = Regex.Replace(slug, @"-+", "-");  
        return slug;
    }
}