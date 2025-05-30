using BlogApp.Dtos.Response;
using BlogApp.Interfaces.Repositories;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using BlogApp.Security;
using BlogApp.Utils;

namespace BlogApp.Services;

public class AuthService(IAuthRepository authRepository, IConfiguration config, IEmailService emailService, ILogger<AuthService> logger) : IAuthService
{
    public async Task<UserResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if(await authRepository.ExistsByEmailAsync(request.Email))
            throw new ApplicationException("Email already exists");
        var registerUser = new User()
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHelper.HashPassword(request.Password),
            EmailVerificationToken = EmailVerificationUtils.GenerateVerificationToken(),
            EmailVerificationTokenExpiresAt = EmailVerificationUtils.GetTokenExpiration(),
            IsEmailVerified = false,
            CreatedAt = DateTime.Now
        };

        var user = await authRepository.CreateAsync(registerUser);
        await SendVerificationEmailAsync(user);
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Token = "Pending verification - Check your email for verification link"
        };
    }

    private async Task SendVerificationEmailAsync(User user)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            logger.LogWarning("User email is empty, cannot send verification email.");
            return;
        }

        try
        {
            var verificationLink = EmailVerificationUtils.GenerateVerificationLink(
                config["Client:BaseUrl"], user.EmailVerificationToken, user.Email);

            var emailBody = EmailTemplateUtils.GetVerificationEmailTemplate(user.Username, verificationLink);

            await emailService.SendEmailAsync(
                user.Email,
                "Verify your email address",
                emailBody
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send verification email to {Email}", user.Email);
            throw new ApplicationException("Failed to send verification email", ex);
        }
    }


    public async Task<bool> VerifyEmailAsync(string email, string token)
    {
        var user = await authRepository.GetByEmailAsync(email.Trim().ToLower());
        if (user == null) throw new ApplicationException("User not found");
        if(user.IsEmailVerified)
            throw new ApplicationException("Email already verified");
        if (user.EmailVerificationToken != token || 
            user.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
        {
            throw new ApplicationException("Invalid or expired verification token");
        }
        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiresAt = null;
        await authRepository.UpdateAsync(user);
        return true;
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
        if(!user.IsEmailVerified) throw new UnauthorizedAccessException("Email not verified. Please check your inbox.");
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

    public async Task<ProfileResponseDto> UpdatePrifileAsync(Guid id, UpdateProfileRequestDto request)
    {
        var user = await authRepository.GetByIdAsync(id);
        if (user == null) return null;
        if(request.Bio != null)
            user.Bio = request.Bio;
        if(request.Avatar != null)
            user.Avatar = request.Avatar;
        user.UpdatedAt = DateTime.Now;
        var updatedUser = await authRepository.UpdateAsync(user);
        return new ProfileResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
    }
}