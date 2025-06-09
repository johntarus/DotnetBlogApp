using BlogApp.Entities;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Repositories;

public interface ILikeRepository
{
    Task<PaginatedList<Like>> GetLikesAsync(int pageNumber, int pageSize);
    Task<Like?> GetLikeByIdAsync(int id);
    Task<Like?> GetLikeAsync(Guid postId, Guid userId);
    Task<Like> AddLikeAsync(Like like);
    Task<bool> RemoveLikeAsync(Guid postId, Guid userId);
    Task<bool> HasUserLikedPostAsync(Guid postId, Guid userId);
}