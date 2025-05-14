namespace BlogApp.Helpers;

public class PasswordHelper
{
    public static byte[] HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        return sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    public static bool VerifyPassword(string password, byte[] storedHash)
    {
        var computedHash = HashPassword(password);
        return computedHash.SequenceEqual(storedHash);
    }

}