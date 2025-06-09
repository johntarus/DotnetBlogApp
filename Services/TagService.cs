using AutoMapper;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;

namespace BlogApp.Services;

public class TagService(ITagRepository tagRepository, IMapper mapper) : ITagService
{
    public async Task<PaginatedList<TagResponseDto>> GetAllTags(int pageNumber, int pageSize)
    {
        var paginatedTags = await tagRepository.GetAllTags(pageNumber, pageSize);
        var tags =  mapper.Map<List<TagResponseDto>>(paginatedTags.Items);
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
        var tag = await tagRepository.GetTagById(id);
        if(tag == null)return null;
        return mapper.Map<TagResponseDto>(tag);
    }

    public async Task<TagResponseDto> AddTag(AddTagDto addTag)
    {
        var createTag = mapper.Map<Tag>(addTag);
        if (createTag == null) return null;
        var tag = await tagRepository.AddTag(createTag);
        return mapper.Map<TagResponseDto>(tag);
    }

    public async Task<TagResponseDto> UpdateTag(int id, UpdateTagDto updateTag)
    {
        var tag = await tagRepository.GetTagById(id);
        if(tag == null) return null;
        mapper.Map(updateTag, tag);
        var updatedTag = await tagRepository.UpdateTag(tag);
        return mapper.Map<TagResponseDto>(updatedTag);
    }

    public async Task<bool> DeleteTag(int id)
    {
        var tag = await tagRepository.GetTagById(id);
        if(tag == null) return false;
        await tagRepository.DeleteTag(tag);
        return true;
    }
}