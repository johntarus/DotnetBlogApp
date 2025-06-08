namespace BlogApp.Models.Dtos;

public class UpdatePostDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? slug { get; set; }
}