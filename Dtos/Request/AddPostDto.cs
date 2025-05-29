namespace BlogApp.Models.Dtos;

public class AddPostDto
{
    public required string Title { get; set; }
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public int CategoryId { get; set; }
    public IEnumerable<int>? TagIds { get; set; }
}