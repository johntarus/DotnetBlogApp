namespace BlogApp.Core.Dtos.Response;

public class LoginRequestDto
{
    public required string UsernameOrEmail { get; set; }
    public required string Password { get; set; }
}