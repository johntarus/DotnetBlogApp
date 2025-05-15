namespace BlogApp.Models.Dtos;

public class LoginRequestDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
}