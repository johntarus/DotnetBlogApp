using System.Net;
using BlogApp.Interfaces.Services;

namespace BlogApp.Utils;

public abstract class EmailVerificationUtils(IConfiguration config, ILogger logger, IEmailService emailService)
{
    public static string GenerateVerificationToken()
    {
        return Guid.NewGuid().ToString("N") + DateTime.UtcNow.Ticks.ToString("x");
    }

    public static string GenerateVerificationLink(string baseUrl, string token, string email)
    {
        return $"{baseUrl}/verify-email?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(email)}";
    }

    public static DateTime GetTokenExpiration(int hoursValid = 24) => DateTime.UtcNow.AddHours(hoursValid);

    
}