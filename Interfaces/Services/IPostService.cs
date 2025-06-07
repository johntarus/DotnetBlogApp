using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface IPostService
{
    Task<IEnumerable<PostResponseDto>> GetPostsAsync(Guid userId);
    Task<PostResponseDto?> GetPostByIdAsync(Guid id);
    Task<PostResponseDto?> CreatePostAsync(AddPostDto dto);
    Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto dto);
    Task<bool> DeletePostAsync(Guid id);
}