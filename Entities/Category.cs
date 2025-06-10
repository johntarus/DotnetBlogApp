using BlogApp.Entities;

namespace BlogApp.Models.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Post> Posts { get; set; } = new List<Post>();
}