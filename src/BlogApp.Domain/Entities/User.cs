using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public string? Bio { get; set; }
    public string? Avatar { get; set; }
    public byte[] PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }
    public string? RefreshToken { get; set; }
    public string Role { get; set; } = "User";
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }
    public List<Post> Posts { get; set; } = new();
    public List<Like> Likes { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}