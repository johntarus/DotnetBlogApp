using BlogApp.Models.Dtos;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<PostResponseDto> Posts { get; set; } = new List<PostResponseDto>();
}