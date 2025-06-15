namespace BlogApp.Domain.Entities;

public class Like
{
    public int Id { get; set; }
    
    // Foreign Keys
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    
    // Navigation properties
    public Post Post { get; set; } = null;
    public User User { get; set; } = null;
    public DateTime CreateAt { get; init; }
}