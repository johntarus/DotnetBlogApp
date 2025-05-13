namespace BlogApp.Models.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string Bio { get; set; }
    public string Avatar { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Post> Posts { get; set; } = new List<Post>();
    public List<Like> Likes { get; set; } = new List<Like>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
}