using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Repositories;

public interface ILikeRepository
{
    Task<PaginatedList<Like>> GetLikesAsync(LikesPagedRequest request);
    Task<Like?> GetLikeByIdAsync(int id);
    Task<Like?> GetLikeAsync(Guid postId, Guid userId);
    Task<Like> AddLikeAsync(Like like);
    Task<bool> RemoveLikeAsync(Guid postId, Guid userId);
    Task<bool> HasUserLikedPostAsync(Guid postId, Guid userId);
}