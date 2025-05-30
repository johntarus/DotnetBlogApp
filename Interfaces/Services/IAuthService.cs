using BlogApp.Dtos.Response;
using BlogApp.Models.Dtos;

namespace BlogApp.Interfaces.Services;

public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<bool> VerifyEmailAsync(string userId, string token);
    Task<UserResponseDto> LoginAsync(LoginRequestDto request);
    Task<ProfileResponseDto> GetProfileAsync(Guid userId);
    Task<ProfileResponseDto> UpdatePrifileAsync(Guid id, UpdateProfileRequestDto request);
}