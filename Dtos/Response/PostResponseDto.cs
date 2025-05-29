namespace BlogApp.Models.Dtos;

public class PostResponseDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string Slug { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public List<string> Tags { get; set; } = new();
}