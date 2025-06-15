using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Services;

public interface ILikeService
{
    Task<PaginatedList<LikeResponseDto>> GetLikesAsync(LikesPagedRequest request);
    Task<LikeResponseDto?> GetLikeByIdAsync(int id);
    Task<LikeResponseDto?> CreateLikeAsync(LikeDto dto);
    Task<bool> RemoveLikeAsync(LikeDto dto);
    Task<bool> CheckIfLikedAsync(CheckLikeRequestDto dto);
}