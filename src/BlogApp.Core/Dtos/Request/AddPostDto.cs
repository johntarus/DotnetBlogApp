namespace BlogApp.Core.Dtos.Request;

public class AddPostDto
{
    public required string Title { get; set; }
    public string? Content { get; set; }
    // public Guid UserId { get; set; } Get the user id from logged in user
    public int CategoryId { get; set; }
    public List<int> TagIds { get; set; }
}