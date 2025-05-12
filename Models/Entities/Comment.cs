namespace BlogApp.Models.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null;
    public Guid UserId { get; set; }
    public User User { get; set; } = null;
}