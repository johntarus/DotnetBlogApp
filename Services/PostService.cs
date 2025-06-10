using AutoMapper;
using BlogApp.Dtos.PagedFilters;
using BlogApp.Dtos.Request;
using BlogApp.Entities;
using BlogApp.Exceptions;
using BlogApp.Helpers;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;

namespace BlogApp.Services;

public class PostService(IPostRepository postRepository, ITagRepository tagRepository, IMapper mapper) : IPostService
{
    public async Task<PaginatedList<PostResponseDto>> GetPostsAsync(Guid userId, bool isAdmin, PostPagedRequest request)
    {
        var paginatedPosts = isAdmin 
            ? await postRepository.GetPostsAsync(request) 
            : await postRepository.GetPostsByUserIdAsync(userId, request.PageNumber, request.PageSize);
        
        var posts = mapper.Map<List<PostResponseDto>>(paginatedPosts.Items);
        return new PaginatedList<PostResponseDto>(
            posts,
            paginatedPosts.PageNumber,
            paginatedPosts.PageSize,
            paginatedPosts.TotalCount,
            paginatedPosts.TotalPages
        );
    }

    public async Task<PostResponseDto?> GetPostByIdAsync(Guid id, Guid userId, string role)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        bool isPostOwner = post.UserId == userId;
        bool isAdmin = role == "Admin";
        if(!isAdmin && !isPostOwner) throw new ForbiddenAccessException();
        if (post == null) return null;
        return mapper.Map<PostResponseDto>(post);
    }

    public async Task<PostResponseDto?> CreatePostAsync(AddPostDto postDto, Guid userId)
    {
        var post = mapper.Map<Post>(postDto);
        post.UserId = userId;
        post.Slug = SlugUtils.GenerateSlug(postDto.Title);
        if (postDto.TagIds != null && postDto.TagIds.Any())
        {
            var allTags = await tagRepository.GetTagsByIds(postDto.TagIds);
            var tags = allTags.Where(t => postDto.TagIds.Contains(t.Id)).ToList();
            post.Tags = tags;
        }
        var createdPost = await postRepository.CreatePostAsync(post);
        return mapper.Map<PostResponseDto>(createdPost);
    }

    public async Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto updatePost, Guid userId,
        string role)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null) return null;
        mapper.Map(updatePost, post);
        bool isPostOwner = post.UserId == userId;
        bool isAdmin = role == "Admin";
        if(!isAdmin && !isPostOwner) throw new ForbiddenAccessException();
        var updatedPost = await postRepository.UpdatePostAsync(post);
        return mapper.Map<PostResponseDto>(updatedPost);

    }

    public async Task<bool> DeletePostAsync(Guid id, Guid userId, string role)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null) return false;
        bool isPostOwner = post.UserId == userId;
        bool isAdmin = role == "Admin";
        if(!isAdmin && !isPostOwner) throw new ForbiddenAccessException();
        await postRepository.DeletePostAsync(post);
        return true;
    }
}