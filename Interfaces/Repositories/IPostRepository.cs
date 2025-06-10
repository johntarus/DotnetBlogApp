using BlogApp.Dtos.PagedFilters;
using BlogApp.Entities;

namespace BlogApp.Interfaces.Repositories;

public interface IPostRepository
{
    Task<PaginatedList<Post>> GetPostsAsync(PostPagedRequest request);
    Task<PaginatedList<Post>> GetPostsByUserIdAsync(Guid userId, int pageNumber, int pageSize);

    Task<Post?> GetPostByIdAsync(Guid id);
    Task<Post> CreatePostAsync(Post post);
    Task<Post> UpdatePostAsync(Post post);
    Task DeletePostAsync(Post post);
}