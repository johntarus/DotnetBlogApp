using BlogApp.Models.Entities;

namespace BlogApp.Models.Dtos;

public class TagResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PostResponseDto> Posts { get; set; } = new();
}