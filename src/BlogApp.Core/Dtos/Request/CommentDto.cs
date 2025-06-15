namespace BlogApp.Core.Dtos.Request;

public class CommentDto
{
    public required string Content { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}