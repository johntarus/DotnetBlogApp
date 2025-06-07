using AutoMapper;
using BlogApp.Helpers;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class PostService(IPostRepository postRepository, ITagRepository tagRepository, IMapper mapper) : IPostService
{
    public async Task<IEnumerable<PostResponseDto>> GetPostsAsync(Guid userId)
    {
        var posts = await postRepository.GetPostsAsync(userId);
        return mapper.Map<IEnumerable<PostResponseDto>>(posts);
    }

    public async Task<PostResponseDto?> GetPostByIdAsync(Guid id)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null) return null;
        return mapper.Map<PostResponseDto>(post);
    }

    public async Task<PostResponseDto?> CreatePostAsync(AddPostDto postDto)
    {
        var post = mapper.Map<Post>(postDto);
        post.Slug = SlugUtils.GenerateSlug(postDto.Title);
        if (postDto.TagIds != null && postDto.TagIds.Any())
        {
            var allTags = await tagRepository.GetAllTags();
            var tags = allTags.Where(t => postDto.TagIds.Contains(t.Id)).ToList();
            post.Tags = tags;
        }
        var createdPost = await postRepository.CreatePostAsync(post);
        return mapper.Map<PostResponseDto>(createdPost);
    }

    public async Task<PostResponseDto?> UpdatePostAsync(Guid id, UpdatePostDto updatePost)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null) return null;
        mapper.Map(updatePost, post);
        var updatedPost = await postRepository.UpdatePostAsync(post);
        return mapper.Map<PostResponseDto>(updatedPost);

    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var post = await postRepository.GetPostByIdAsync(id);
        if (post == null) return false;
        await postRepository.DeletePostAsync(post);
        return true;
    }
}