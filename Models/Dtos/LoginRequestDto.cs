namespace BlogApp.Models.Dtos;

public class LoginRequestDto
{
    public required string UsernameOrEmail { get; set; }
    public required string Password { get; set; }
}