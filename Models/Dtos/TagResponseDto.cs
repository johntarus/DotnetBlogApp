using BlogApp.Models.Entities;

namespace BlogApp.Models.Dtos;

public class TagResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Post> Posts { get; set; } = new List<Post>();
}