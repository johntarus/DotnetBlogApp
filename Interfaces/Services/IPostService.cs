using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface IPostService
{
    Task<IEnumerable<PostResponseDto>> GetPostsAsync();
    Task<PostResponseDto?> GetPostByIdAsync(int id);
    Task<PostResponseDto?> CreatePostAsync(PostDto dto);
    Task<PostResponseDto?> UpdatePostAsync(int id, PostDto dto);
    Task<bool> DeletePostAsync(int id);
}