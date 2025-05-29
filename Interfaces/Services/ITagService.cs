using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Services;

public interface ITagService
{
    Task<IEnumerable<TagResponseDto>> GetAllTags();
    Task<TagResponseDto> GetTagById(int id);
    Task<TagResponseDto> AddTag(TagResponseDto tag);
    Task<TagResponseDto> UpdateTag(TagResponseDto tag);
    Task<bool> DeleteTag(int id);
}