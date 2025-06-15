namespace BlogApp.Core.Dtos.Request;

public class CheckLikeRequestDto
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
}