using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;

namespace BlogApp.Core.Interfaces.Services;

public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<bool> VerifyEmailAsync(string userId, string token);
    Task<UserResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ProfileResponseDto> GetProfileAsync(Guid userId);
    Task<ProfileResponseDto> UpdateProfileAsync(Guid id, UpdateProfileRequestDto request);
}