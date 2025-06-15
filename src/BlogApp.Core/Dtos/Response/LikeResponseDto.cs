namespace BlogApp.Core.Dtos.Response;

public class LikeResponseDto
{
    public int Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedAt { get; set; }
}