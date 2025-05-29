using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface IPostService
{
    Task<IEnumerable<PostResponseDto>> GetPostsAsync();
    Task<PostResponseDto?> GetPostByIdAsync(Guid id);
    Task<PostDto?> CreatePostAsync(AddPostDto dto);
    Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto dto);
    Task<bool> DeletePostAsync(Guid id);
}