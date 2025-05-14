using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models.Dtos;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    [EmailAddress]
    public string Email { get; set; } = "";
    public string Bio { get; set; } = "";
    public string Avatar { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string Token { get; set; } = "";
}