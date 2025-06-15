namespace BlogApp.Core.Dtos.Request;

public class UpdatePostDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? slug { get; set; }
}