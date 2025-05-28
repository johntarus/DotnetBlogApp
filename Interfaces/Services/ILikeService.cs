using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface ILikeService
{
    Task<List<LikeResponseDto>> GetLikesAsync();
    Task<LikeResponseDto> GetLikeByIdAsync(int id);
    Task<LikeResponseDto> CreateLikeAsync(LikeDto likeDto);
    Task<LikeResponseDto> UpdateLikeAsync(int id, LikeDto likeDto);
    Task DeleteLikeAsync(int id);
}