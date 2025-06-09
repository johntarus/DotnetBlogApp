using BlogApp.Dtos.Response;
using BlogApp.Entities;
using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface ILikeService
{
    Task<PaginatedList<LikeResponseDto>> GetLikesAsync(int pageNumber, int pageSize);
    Task<LikeResponseDto?> GetLikeByIdAsync(int id);
    Task<LikeResponseDto?> CreateLikeAsync(LikeDto dto);
    Task<bool> RemoveLikeAsync(LikeDto dto);
    Task<bool> CheckIfLikedAsync(CheckLikeRequestDto dto);
}