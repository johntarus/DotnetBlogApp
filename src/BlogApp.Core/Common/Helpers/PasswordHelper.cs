using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Core.Common.Helpers;

public static class PasswordHelper
{
    public static (byte[] hash, byte[] salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA256();
        return (
            hash: hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            salt: hmac.Key
        );
    }

    public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA256(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }
}
