namespace BlogApp.Core.Dtos.Response;

public class CommentResponseDto
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsEdited { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; }
}