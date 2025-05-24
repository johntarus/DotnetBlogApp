using BlogApp.Interfaces;
using BlogApp.Models.Dtos;

namespace BlogApp.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
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
}