using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;

namespace BlogApp.Services;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public async Task<IEnumerable<TagResponseDto>> GetAllTags()
    {
        var tags = await tagRepository.GetTags();
        var tagsResponse = tags.Select(t => new TagResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            CreateAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            Posts = t.Posts.Select(p=> new PostResponseDto
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
                Tags = p.Tags.Select(t => t.Name).ToList()
            }).ToList()
        });
        return tagsResponse;
    }

    public async Task<TagResponseDto> GetTagById(int id)
    {
        var tag = await tagRepository.GetTagById(id);
        return new TagResponseDto
        {
            Id = tag.Id,
            Name = tag.Name,
            CreateAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt,
            Posts = tag.Posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Username = p.User?.Username,
                CategoryName = p.Categories?.Name,
                LikesCount = p.Likes?.Count() ?? 0,
                CommentsCount = p.Comments?.Count() ?? 0,
                Tags = p.Tags.Select(t => t.Name).ToList()
            }).ToList()
        };
    }

    public async Task<TagResponseDto> AddTag(AddTagDto addTag)
    {
        var createTag = new Tag()
        {
            Name = addTag.Name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
        if (createTag == null) return null;
        var tag = await tagRepository.AddTag(createTag);
        return new TagResponseDto()
        {
            Id = tag.Id,
            Name = tag.Name,
            CreateAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt
        };
    }

    public async Task<TagResponseDto> UpdateTag(int id, UpdateTagDto updateTag)
    {
        var tag = await tagRepository.GetTagById(id);
        if(tag == null) return null;
        if(!string.IsNullOrWhiteSpace(updateTag.Name))
            tag.Name = updateTag.Name;
        var updatedTag = await tagRepository.UpdateTag(tag);
        return new TagResponseDto()
        {
            Id = updatedTag.Id,
            Name = updatedTag.Name,
            CreateAt = updatedTag.CreatedAt,
            UpdatedAt = updatedTag.UpdatedAt
        };
    }

    public Task<bool> DeleteTag(int id)
    {
        throw new NotImplementedException();
    }
}