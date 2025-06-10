using BlogApp.Models.Entities;

namespace BlogApp.Entities;

public class Post
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
    public string Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Like> Likes { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public IEnumerable<Tag> Tags { get; set; } = new List<Tag>();
}