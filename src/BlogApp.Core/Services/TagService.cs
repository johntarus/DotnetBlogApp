using System.Text.Json;
using AutoMapper;
using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Dtos.Request;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BlogApp.Core.Services;

public class TagService(ITagRepository tagRepository, IMapper mapper, ILogger<TagService> logger) : ITagService
{
    public async Task<PaginatedList<TagResponseDto>> GetAllTags(TagPagedRequest request)
    {
        logger.LogInformation("Fetching all tags with request: {request}", JsonSerializer.Serialize(request));
        var paginatedTags = await tagRepository.GetAllTags(request);
        var tags = mapper.Map<List<TagResponseDto>>(paginatedTags.Items);
        logger.LogInformation("Fetched {count} tags", tags.Count);
        return new PaginatedList<TagResponseDto>(
            tags,
            paginatedTags.PageNumber,
            paginatedTags.PageSize,
            paginatedTags.TotalCount,
            paginatedTags.TotalPages
        );
    }

    public async Task<TagResponseDto> GetTagById(int id)
    {
        logger.LogInformation("Fetching tag with id={id}", id);
        var tag = await tagRepository.GetTagById(id);
        if (tag == null)
        {
            logger.LogWarning("Tag with id={id} not found", id);
            return null;
        }
        logger.LogInformation("Returning tag: {tag}", JsonSerializer.Serialize(tag));
        return mapper.Map<TagResponseDto>(tag);
    }

    public async Task<TagResponseDto> AddTag(AddTagDto addTag)
    {
        logger.LogInformation("Adding tag: {addTag}", JsonSerializer.Serialize(addTag));
        var createTag = mapper.Map<Tag>(addTag);
        if (createTag == null)
        {
            logger.LogWarning("Mapping failed for AddTagDto: {addTag}", JsonSerializer.Serialize(addTag));
            return null;
        }
        var tag = await tagRepository.AddTag(createTag);
        logger.LogInformation("Tag added: {tag}", JsonSerializer.Serialize(tag));
        return mapper.Map<TagResponseDto>(tag);
    }

    public async Task<TagResponseDto> UpdateTag(int id, UpdateTagDto updateTag)
    {
        logger.LogInformation("Updating tag id={id} with data: {updateTag}", id, JsonSerializer.Serialize(updateTag));
        var tag = await tagRepository.GetTagById(id);
        if (tag == null)
        {
            logger.LogWarning("Tag with id={id} not found", id);
            return null;
        }
        mapper.Map(updateTag, tag);
        var updatedTag = await tagRepository.UpdateTag(tag);
        logger.LogInformation("Tag updated: {tag}", JsonSerializer.Serialize(updatedTag));
        return mapper.Map<TagResponseDto>(updatedTag);
    }

    public async Task<bool> DeleteTag(int id)
    {
        logger.LogInformation("Deleting tag with id={id}", id);
        var tag = await tagRepository.GetTagById(id);
        if (tag == null)
        {
            logger.LogWarning("Tag with id={id} not found for deletion", id);
            return false;
        }
        await tagRepository.DeleteTag(tag);
        logger.LogInformation("Tag with id={id} deleted", id);
        return true;
    }
}
