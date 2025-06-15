using BlogApp.Core.Dtos.PagedFilters;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Dtos.Request;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Interfaces.Services;

public interface ITagService
{
    Task<PaginatedList<TagResponseDto>> GetAllTags(TagPagedRequest request);
    Task<TagResponseDto> GetTagById(int id);
    Task<TagResponseDto> AddTag(AddTagDto addTag);
    Task<TagResponseDto> UpdateTag(int id, UpdateTagDto tag);
    Task<bool> DeleteTag(int id);
}