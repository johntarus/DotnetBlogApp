namespace BlogApp.Core.Dtos.Response;

public class ProfileResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Bio { get; set; } = "";
    public string? Avatar { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}