using System.ComponentModel.DataAnnotations;

namespace BlogApp.Core.Dtos.Response;

public class UserResponseDto
{
    public Guid Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}