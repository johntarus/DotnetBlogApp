using BlogApp.Helpers;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class PostService(IPostRepository postRepository, ITagRepository tagRepository) : IPostService
{
    public async Task<IEnumerable<PostResponseDto>> GetPostsAsync()
    {
        var posts = await postRepository.GetPostsAsync();
        return posts.Select(p => new PostResponseDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            Slug = p.Slug,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
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

    public async Task<PostResponseDto?> CreatePostAsync(AddPostDto postDto)
    {
        var post = new Post
        {
            Title = postDto.Title,
            Content = postDto.Content,
            Slug = SlugUtils.GenerateSlug(postDto.Title),
            CategoryId = postDto.CategoryId,
            UserId = postDto.UserId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        if (postDto.TagIds != null && postDto.TagIds.Any())
        {
            var allTags = await tagRepository.GetAllTags();
            var tags = allTags.Where(t => postDto.TagIds.Contains(t.Id)).ToList();
            post.Tags = tags;
        }
        var createdPost = await postRepository.CreatePostAsync(post);
        return new PostResponseDto
        {
            Id = createdPost.Id,
            Title = createdPost.Title,
            Content = createdPost.Content,
            Slug = createdPost.Slug,
            CreatedAt = createdPost.CreatedAt,
            UpdatedAt = createdPost.UpdatedAt,
            CategoryId = createdPost.CategoryId,
            CategoryName = createdPost.Categories?.Name,
            UserId = createdPost.UserId,
            Username = createdPost.User?.Username,
            LikesCount = createdPost.Likes?.Count ?? 0,
            CommentsCount = createdPost.Comments?.Count ?? 0,
            Tags = createdPost.Tags.Select(t => t.Name).ToList()
        };
    }

    public async Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto dto)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null) return null;
        if(!string.IsNullOrEmpty(dto.Title))
            post.Title = dto.Title;
        if(!string.IsNullOrEmpty(dto.Content))
            post.Content = dto.Content;
        var updatedPost = await postRepository.UpdatePostAsync(post);
        return new PostResponseDto()
        {
            Id = updatedPost.Id,
            Title = updatedPost.Title,
            CreatedAt = updatedPost.CreatedAt,
            UpdatedAt = updatedPost.UpdatedAt,
            Content = updatedPost.Content,
            CategoryId = updatedPost.CategoryId,
            CategoryName = updatedPost.Categories?.Name,
            UserId = updatedPost.UserId,
            Username = updatedPost.User?.Username,
            Tags = updatedPost.Tags.Select(t => t.Name).ToList()
        };

    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null) return false;
        await postRepository.DeletePostAsync(post);
        return true;
    }
}