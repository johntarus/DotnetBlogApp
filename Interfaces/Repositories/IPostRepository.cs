using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Repositories;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetPostsAsync();
    Task<Post?> GetPostByIdAsync(int id);
    Task<Post> CreatePostAsync(Post post);
    Task<Post> UpdatePostAsync(Post post);
    Task DeletePostAsync(Post post);
}