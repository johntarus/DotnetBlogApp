using BlogApp.Models.Entities;

namespace BlogApp.Models.Dtos;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
}