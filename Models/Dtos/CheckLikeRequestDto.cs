namespace BlogApp.Models.Dtos;

public class CheckLikeRequestDto
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
}