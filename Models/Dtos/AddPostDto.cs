namespace BlogApp.Models.Dtos;

public class AddPostDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}