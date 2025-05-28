using BlogApp.Models.Entities;

namespace BlogApp.Interfaces;

public interface ILikeRepository
{
    Task<IEnumerable<Like>> GetLikesAsync();
    Task<Like> GetLikeByIdAsync(int id);
    Task<Like> CreateLikeAsync(Like like);
    Task<Like> UpdateLikeAsync(Like like);
    Task<bool> DeleteLikeAsync(Like like);
}