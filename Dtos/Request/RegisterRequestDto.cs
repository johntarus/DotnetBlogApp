using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models.Dtos;

public class RegisterRequestDto
{
    public required string Username { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}