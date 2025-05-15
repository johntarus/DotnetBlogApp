namespace BlogApp.Models.Dtos;

public class CommentDto
{
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}