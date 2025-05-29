using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using BlogApp.Security;

namespace BlogApp.Services;

public class AuthService(IAuthRepository authRepository, IConfiguration config) : IAuthService
{
    public async Task<UserResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if(await authRepository.ExistsByEmailAsync(request.Email))
            throw new ApplicationException("Email already exists");
        var registerUser = new User()
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHelper.HashPassword(request.Password)
        };

        var user = await authRepository.CreateAsync(registerUser);
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Token = JwtHelper.GenerateToken(user, config)
        };
    }

    public async Task<UserResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await authRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail);
        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
            throw new ArgumentException("Username or Email is required");
        if(string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required");
        if(user==null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid Credentials");
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Token = JwtHelper.GenerateToken(user, config)
        };
    }

    public async Task<ProfileResponseDto> GetProfileAsync(Guid userId)
    {
        var user = await authRepository.GetByIdAsync(userId);
        if (user == null) throw new ApplicationException("User not found");
        return new ProfileResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public Task<UpdateProfileRequestDto> UpdatePrifileAsync(Guid id, UpdateProfileRequestDto request)
    {
        throw new NotImplementedException();
    }
}