namespace BlogApp.Models.Entities;

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
    public Category Categories { get; set; }
    public List<Like> Likes { get; set; } = new List<Like>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<Tag> Tags { get; set; } = new List<Tag>();
}