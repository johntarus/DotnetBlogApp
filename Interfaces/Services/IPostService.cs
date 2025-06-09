using BlogApp.Dtos.Request;
using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface IPostService
{
    Task<IEnumerable<PostResponseDto>> GetPostsAsync(Guid userId, bool isAdmin);
    Task<PostResponseDto?> GetPostByIdAsync(Guid id, Guid userId, string role);
    Task<PostResponseDto?> CreatePostAsync(AddPostDto dto, Guid userId);
    Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto dto, Guid userId, string role);
    Task<bool> DeletePostAsync(Guid id, Guid userId, string role);
}