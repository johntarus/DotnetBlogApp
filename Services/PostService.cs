using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;

namespace BlogApp.Services;

public class PostService(IPostRepository postRepository) : IPostService
{
    public async Task<IEnumerable<PostResponseDto>> GetPostsAsync()
    {
        var posts = await postRepository.GetPostsAsync();
        return posts.Select(p => new PostResponseDto
        {
            Id = p.Id,
            Title = p.Title,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Content = p.Content,
            CategoryId = p.CategoryId,
            CategoryName = p.Categories?.Name,
            UserId = p.UserId,
            Username = p.User?.Username,
            LikesCount = p.Likes?.Count ?? 0,
            CommentsCount = p.Comments?.Count ?? 0,
            Tags = p.Tags.Select(t => t.Name).ToList()
        }).ToList();
    }

    public async Task<PostResponseDto?> GetPostByIdAsync(Guid id)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null) return null;
        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Content = post.Content,
            CategoryId = post.CategoryId,
            CategoryName = post.Categories?.Name,
            UserId = post.UserId,
            Username = post.User?.Username,
            LikesCount = post.Likes?.Count ?? 0,
            CommentsCount = post.Comments?.Count ?? 0,
            Tags = post.Tags.Select(t => t.Name).ToList()
        };
    }

    public Task<PostResponseDto?> CreatePostAsync(PostDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<PostResponseDto?> UpdatePostAsync(int id, PostDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePostAsync(int id)
    {
        throw new NotImplementedException();
    }
}