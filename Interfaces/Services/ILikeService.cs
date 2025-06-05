using BlogApp.Dtos.Response;
using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface ILikeService
{
    Task<List<LikeResponseDto>> GetLikesAsync();
    Task<LikeResponseDto?> GetLikeByIdAsync(int id);
    Task<LikeResponseDto?> CreateLikeAsync(LikeDto dto);
    Task<bool> RemoveLikeAsync(LikeDto dto);
    Task<bool> CheckIfLikedAsync(CheckLikeRequestDto dto);
}