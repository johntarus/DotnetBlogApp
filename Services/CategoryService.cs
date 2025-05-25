using BlogApp.Interfaces;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<List<CategoryResponseDto>> GetCategoriesAsync()
    {
        var categories = await categoryRepository.GetCategoriesAsync();

        return categories.Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Posts = c.Posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CategoryId = p.CategoryId,
                CategoryName = c.Name,
                UserId = p.UserId,
                Username = p.User.Username,  
                LikesCount = p.Likes.Count,
                CommentsCount = p.Comments.Count,
                Tags = p.Tags.Select(t => t.Name).ToList()
            }).ToList()
        }).ToList();
    }


    public async Task<CategoryResponseDto?> GetCategoryById(int id)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id);
        if (category == null) return null;
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Posts = category.Posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Categories.Name,
                UserId = p.UserId,
                Username = p.User.Username,
                LikesCount = p.Likes.Count,
                CommentsCount = p.Comments.Count,
                Tags = p.Tags.Select(t => t.Name).ToList()
            }).ToList()
        };
    }

    public async Task<Category> AddCategoryAsync(AddCategoryDto categoryDto)
    {
        var category = new Category()
        {
            Name = categoryDto.Name
        };
        return await categoryRepository.CreateCategoryAsync(category);
    }

    public Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteCategoryAsync(int id)
    {
        throw new NotImplementedException();
    }
}