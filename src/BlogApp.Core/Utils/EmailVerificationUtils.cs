using System.Net;

namespace BlogApp.Core.Utils;

public static class EmailVerificationUtils
{
    public static string GenerateVerificationToken()
    {
        return Guid.NewGuid().ToString("N") + DateTime.UtcNow.Ticks.ToString("x");
    }

    public static string GenerateVerificationLink(string baseUrl, string token, string email)
    {
        return $"{baseUrl}/verify-email?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(email)}";
    }

    public static DateTime GetTokenExpiration(int hoursValid = 24)
    {
        return DateTime.UtcNow.AddHours(hoursValid);
    }
}