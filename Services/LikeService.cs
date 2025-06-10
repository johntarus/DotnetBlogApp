using System.Text.Json;
using AutoMapper;
using BlogApp.Dtos.PagedFilters;
using BlogApp.Dtos.Response;
using BlogApp.Entities;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class LikeService(
    ILikeRepository likeRepo,
    IAuthRepository authRepository,
    IPostRepository postRepository,
    IMapper mapper,
    ILogger<LikeService> logger) : ILikeService
{
    public async Task<PaginatedList<LikeResponseDto>> GetLikesAsync(LikesPagedRequest request)
    {
        logger.LogInformation("Fetching likes: {Request}", JsonSerializer.Serialize(request));
        var paginatedLikes = await likeRepo.GetLikesAsync(request);
        var likes = mapper.Map<List<LikeResponseDto>>(paginatedLikes.Items);
        var response = new PaginatedList<LikeResponseDto>(
            likes,
            paginatedLikes.PageNumber,
            paginatedLikes.PageSize,
            paginatedLikes.TotalCount,
            paginatedLikes.TotalPages
        );
        logger.LogInformation("Likes fetched: {Response}", JsonSerializer.Serialize(response));
        return response;
    }

    public async Task<LikeResponseDto?> GetLikeByIdAsync(int id)
    {
        logger.LogInformation("Fetching like by ID: {Id}", id);
        var like = await likeRepo.GetLikeByIdAsync(id);
        var dto = mapper.Map<LikeResponseDto>(like);
        logger.LogInformation("Fetched like: {Dto}", JsonSerializer.Serialize(dto));
        return dto;
    }

    public async Task<LikeResponseDto?> CreateLikeAsync(LikeDto dto)
    {
        logger.LogInformation("Creating like: {Dto}", JsonSerializer.Serialize(dto));
        var existingLike = await likeRepo.GetLikeAsync(dto.PostId, dto.UserId);
        if (existingLike != null)
        {
            logger.LogWarning("User {UserId} already liked post {PostId}", dto.UserId, dto.PostId);
            return null;
        }

        var user = await authRepository.GetUserByIdAsync(dto.UserId);
        if (user == null)
        {
            logger.LogError("User not found: {UserId}", dto.UserId);
            throw new KeyNotFoundException("User not found");
        }

        var post = await postRepository.GetPostByIdAsync(dto.PostId);
        if (post == null)
        {
            logger.LogError("Post not found: {PostId}", dto.PostId);
            throw new KeyNotFoundException("Post not found");
        }

        var like = mapper.Map<Like>(dto);
        var created = await likeRepo.AddLikeAsync(like);
        var responseDto = mapper.Map<LikeResponseDto>(created);

        logger.LogInformation("Like created: {Response}", JsonSerializer.Serialize(responseDto));
        return responseDto;
    }

    public async Task<bool> RemoveLikeAsync(LikeDto dto)
    {
        logger.LogInformation("Removing like: {Dto}", JsonSerializer.Serialize(dto));
        var result = await likeRepo.RemoveLikeAsync(dto.PostId, dto.UserId);
        logger.LogInformation("Remove result: {Result}", result);
        return result;
    }

    public async Task<bool> CheckIfLikedAsync(CheckLikeRequestDto dto)
    {
        logger.LogInformation("Checking if liked: {Dto}", JsonSerializer.Serialize(dto));
        var result = await likeRepo.HasUserLikedPostAsync(dto.PostId, dto.UserId);
        logger.LogInformation("Check result: {Result}", result);
        return result;
    }
}
