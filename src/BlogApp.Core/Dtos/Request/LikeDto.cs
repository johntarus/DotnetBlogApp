namespace BlogApp.Core.Dtos.Request;

public class LikeDto
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }

}