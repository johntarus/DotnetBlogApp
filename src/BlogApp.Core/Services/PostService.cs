using System.Text.Json;
using AutoMapper;
using BlogApp.Core.Common.Exceptions;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Core.Utils;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Services;

public class PostService(
    IPostRepository postRepository,
    ITagRepository tagRepository,
    IMapper mapper,
    ILogger<PostService> logger)
    : IPostService
{
    public async Task<PaginatedList<PostResponseDto>> GetPostsAsync(Guid userId, bool isAdmin, PostPagedRequest request)
    {
        logger.LogInformation("Retrieving posts for user {UserId} with admin={IsAdmin} and request={Request}", userId, isAdmin, JsonSerializer.Serialize(request));

        var paginatedPosts = isAdmin
            ? await postRepository.GetPostsAsync(request)
            : await postRepository.GetPostsByUserIdAsync(userId, request.PageNumber, request.PageSize);

        var posts = mapper.Map<List<PostResponseDto>>(paginatedPosts.Items);

        var result = new PaginatedList<PostResponseDto>(
            posts,
            paginatedPosts.PageNumber,
            paginatedPosts.PageSize,
            paginatedPosts.TotalCount,
            paginatedPosts.TotalPages
        );

        logger.LogInformation("Returning posts: {Result}", JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<PostResponseDto?> GetPostByIdAsync(Guid id, Guid userId, string role)
    {
        logger.LogInformation("Fetching post with ID {PostId} for user {UserId} (Role: {Role})", id, userId, role);
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            logger.LogWarning("Post with ID {PostId} not found", id);
            return null;
        }

        bool isPostOwner = post.UserId == userId;
        bool isAdmin = role == "Admin";

        if (!isAdmin && !isPostOwner)
        {
            logger.LogWarning("Access denied for user {UserId} to post {PostId}", userId, id);
            throw new ForbiddenAccessException();
        }

        var result = mapper.Map<PostResponseDto>(post);
        logger.LogInformation("Returning post: {Result}", JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<PostResponseDto?> CreatePostAsync(AddPostDto postDto, Guid userId)
    {
        logger.LogInformation("Creating new post for user {UserId} with data: {PostDto}", userId, JsonSerializer.Serialize(postDto));

        var post = mapper.Map<Post>(postDto);
        post.UserId = userId;
        post.Slug = SlugUtils.GenerateSlug(postDto.Title);

        if (postDto.TagIds != null && postDto.TagIds.Any())
        {
            var allTags = await tagRepository.GetTagsByIds(postDto.TagIds);
            post.Tags = allTags.Where(t => postDto.TagIds.Contains(t.Id)).ToList();
        }

        var createdPost = await postRepository.CreatePostAsync(post);
        var result = mapper.Map<PostResponseDto>(createdPost);
        logger.LogInformation("Created post: {Result}", JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto updatePost, Guid userId, string role)
    {
        logger.LogInformation("Updating post {PostId} by user {UserId} with data: {UpdateDto}", id, userId, JsonSerializer.Serialize(updatePost));

        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            logger.LogWarning("Post with ID {PostId} not found", id);
            return null;
        }

        bool isPostOwner = post.UserId == userId;
        bool isAdmin = role == "Admin";

        if (!isAdmin && !isPostOwner)
        {
            logger.LogWarning("Access denied for user {UserId} to update post {PostId}", userId, id);
            throw new ForbiddenAccessException();
        }

        mapper.Map(updatePost, post);
        var updatedPost = await postRepository.UpdatePostAsync(post);
        var result = mapper.Map<PostResponseDto>(updatedPost);
        logger.LogInformation("Updated post: {Result}", JsonSerializer.Serialize(result));
        return result;
    }

    public async Task<bool> DeletePostAsync(Guid id, Guid userId, string role)
    {
        logger.LogInformation("User {UserId} (Role: {Role}) is deleting post {PostId}", userId, role, id);

        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null)
        {
            logger.LogWarning("Post with ID {PostId} not found", id);
            return false;
        }

        bool isPostOwner = post.UserId == userId;
        bool isAdmin = role == "Admin";

        if (!isAdmin && !isPostOwner)
        {
            logger.LogWarning("Access denied for user {UserId} to delete post {PostId}", userId, id);
            throw new ForbiddenAccessException();
        }

        await postRepository.DeletePostAsync(post);
        logger.LogInformation("Post with ID {PostId} successfully deleted", id);
        return true;
    }
}