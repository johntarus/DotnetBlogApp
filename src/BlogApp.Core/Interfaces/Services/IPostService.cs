using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Services;

public interface IPostService
{
    Task<PaginatedList<PostResponseDto>> GetPostsAsync(Guid userId, bool isAdmin, PostPagedRequest request);
    Task<PostResponseDto?> GetPostByIdAsync(Guid id, Guid userId, string role);
    Task<PostResponseDto?> CreatePostAsync(AddPostDto dto, Guid userId);
    Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto dto, Guid userId, string role);
    Task<bool> DeletePostAsync(Guid id, Guid userId, string role);
}