using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Repositories;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetPostsAsync();
    Task<IEnumerable<Post>> GetPostsByUserIdAsync(Guid userId);

    Task<Post?> GetPostByIdAsync(Guid id);
    Task<Post> CreatePostAsync(Post post);
    Task<Post> UpdatePostAsync(Post post);
    Task DeletePostAsync(Post post);
}