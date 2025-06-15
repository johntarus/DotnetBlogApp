using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Repositories;

public interface IPostRepository
{
    Task<PaginatedList<Post>> GetPostsAsync(PostPagedRequest request);
    Task<PaginatedList<Post>> GetPostsByUserIdAsync(Guid userId, int pageNumber, int pageSize);

    Task<Post?> GetPostByIdAsync(Guid id);
    Task<Post> CreatePostAsync(Post post);
    Task<Post> UpdatePostAsync(Post post);
    Task DeletePostAsync(Post post);
}