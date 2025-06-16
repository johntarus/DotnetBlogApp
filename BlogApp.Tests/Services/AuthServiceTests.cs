using System.Security.Claims;
using AutoMapper;
using BlogApp.Core.Common.Helpers;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Core.Services;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace BlogApp.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockRepo = new Mock<IAuthRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockLogger = new Mock<ILogger<AuthService>>();
            _mockMapper = new Mock<IMapper>();

            // Setup mock configuration with test JWT values
            var configMock = new Mock<IConfiguration>();
            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns("TestSecretKeyMustBeAtLeast32BytesLong");
            configMock.Setup(x => x["Jwt:Key"]).Returns("TestSecretKeyMustBeAtLeast32BytesLong");
            configMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            configMock.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");
            _mockConfig = configMock;

            _service = new AuthService(
                _mockRepo.Object,
                _mockConfig.Object,
                _mockEmailService.Object,
                _mockLogger.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ReturnsUserResponse_WhenSuccessful()
        {
            // Arrange
            var request = new RegisterRequestDto 
            { 
                Email = "test@example.com", 
                Username = "testuser",
                Password = "Password123!" 
            };
            var user = new User 
            { 
                Id = Guid.NewGuid(), 
                Email = request.Email,
                Username = request.Username
            };
            var userResponse = new UserResponseDto 
            { 
                Id = user.Id,
                Email = user.Email,
                Username = user.Username
            };

            _mockRepo.Setup(r => r.ExistsByEmailAsync(request.Email))
                .ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<User>(request))
                .Returns(user);
            _mockRepo.Setup(r => r.CreateAsync(user))
                .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserResponseDto>(user))
                .Returns(userResponse);

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
            _mockEmailService.Verify(e => e.SendEmailAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()), 
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ThrowsException_WhenEmailExists()
        {
            // Arrange
            var request = new RegisterRequestDto 
            { 
                Email = "exists@example.com", 
                Username = "testuser",
                Password = "Password123!" 
            };

            _mockRepo.Setup(r => r.ExistsByEmailAsync(request.Email))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _service.RegisterAsync(request));
        }

        [Fact]
        public async Task VerifyEmailAsync_ReturnsTrue_WhenValid()
        {
            // Arrange
            var token = "valid_token";
            var email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                EmailVerificationToken = token,
                EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(1),
                IsEmailVerified = false,
                Username = null
            };

            _mockRepo.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _service.VerifyEmailAsync(token, email);

            // Assert
            Assert.True(result);
            Assert.True(user.IsEmailVerified);
            Assert.Null(user.EmailVerificationToken);
            _mockRepo.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task VerifyEmailAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var token = "valid_token";
            var email = "notfound@example.com";

            _mockRepo.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _service.VerifyEmailAsync(token, email));
        }

        [Fact]
        public async Task VerifyEmailAsync_ThrowsException_WhenAlreadyVerified()
        {
            // Arrange
            var token = "valid_token";
            var email = "verified@example.com";
            var user = new User
            {
                Email = email,
                IsEmailVerified = true,
                Username = null
            };

            _mockRepo.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _service.VerifyEmailAsync(token, email));
        }

        [Fact]
        public async Task LoginAsync_ReturnsUserResponse_WhenValid()
        {
            // Arrange
            var request = new LoginRequestDto 
            { 
                UsernameOrEmail = "test@example.com",
                Password = "Password123!" 
            };
            var user = new User 
            { 
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = PasswordHelper.HashPassword("Password123!"),
                IsEmailVerified = true
            };
            var userResponse = new UserResponseDto 
            { 
                Id = user.Id,
                Email = user.Email,
                Username = user.Username
            };

            _mockRepo.Setup(r => r.GetByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserResponseDto>(user))
                .Returns(userResponse);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ThrowsException_WhenInvalidCredentials()
        {
            // Arrange
            var request = new LoginRequestDto 
            { 
                UsernameOrEmail = "test@example.com",
                Password = "WrongPassword" 
            };
            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = PasswordHelper.HashPassword("Password123!"),
                Username = null
            };

            _mockRepo.Setup(r => r.GetByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ThrowsException_WhenEmailNotVerified()
        {
            // Arrange
            var request = new LoginRequestDto 
            { 
                UsernameOrEmail = "test@example.com",
                Password = "Password123!" 
            };
            var user = new User
            {
                Email = "test@example.com",
                PasswordHash = PasswordHelper.HashPassword("Password123!"),
                IsEmailVerified = false,
                Username = null
            };

            _mockRepo.Setup(r => r.GetByUsernameOrEmailAsync(request.UsernameOrEmail))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.LoginAsync(request));
        }

        // [Fact]
        // public async Task RefreshTokenAsync_ReturnsNewTokens_WhenValid()
        // {
        //     // Arrange
        //     var oldAccessToken = JwtHelper.GenerateAccessToken(new User
        //     {
        //         Id = Guid.NewGuid(),
        //         Username = null,
        //         Email = null
        //     }, _mockConfig.Object);
        //     var oldRefreshToken = "old_refresh_token";
        //     var request = new RefreshTokenRequestDto 
        //     { 
        //         AccessToken = oldAccessToken,
        //         RefreshToken = oldRefreshToken
        //     };
        //
        //     var principle = JwtHelper.GetPrincipleFromExpiredToken(oldAccessToken, _mockConfig.Object);
        //     var userId = Guid.Parse(principle.FindFirst(ClaimTypes.NameIdentifier).Value);
        //     
        //     var user = new User
        //     {
        //         Id = userId,
        //         RefreshToken = oldRefreshToken,
        //         RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1),
        //         Username = null,
        //         Email = null
        //     };
        //     var userResponse = new UserResponseDto 
        //     { 
        //         Id = user.Id
        //     };
        //
        //     _mockRepo.Setup(r => r.GetUserByIdAsync(userId))
        //         .ReturnsAsync(user);
        //     _mockMapper.Setup(m => m.Map<UserResponseDto>(user))
        //         .Returns(userResponse);
        //
        //     // Act
        //     var result = await _service.RefreshTokenAsync(request);
        //
        //     // Assert
        //     Assert.NotNull(result);
        //     Assert.NotNull(result.AccessToken);
        //     Assert.NotNull(result.RefreshToken);
        //     Assert.NotEqual(oldRefreshToken, result.RefreshToken);
        //     _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        // }
        //
        // [Fact]
        // public async Task RefreshTokenAsync_ThrowsException_WhenInvalidRefreshToken()
        // {
        //     // Arrange
        //     var oldAccessToken = JwtHelper.GenerateAccessToken(new User
        //     {
        //         Id = Guid.NewGuid(),
        //         Username = null,
        //         Email = null
        //     }, _mockConfig.Object);
        //     var request = new RefreshTokenRequestDto 
        //     { 
        //         AccessToken = oldAccessToken,
        //         RefreshToken = "invalid_token"
        //     };
        //
        //     var principle = JwtHelper.GetPrincipleFromExpiredToken(oldAccessToken, _mockConfig.Object);
        //     var userId = Guid.Parse(principle.FindFirst(ClaimTypes.NameIdentifier).Value);
        //     
        //     var user = new User
        //     {
        //         Id = userId,
        //         RefreshToken = "different_token",
        //         RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1),
        //         Username = null,
        //         Email = null
        //     };
        //
        //     _mockRepo.Setup(r => r.GetUserByIdAsync(userId))
        //         .ReturnsAsync(user);
        //
        //     // Act & Assert
        //     await Assert.ThrowsAsync<SecurityTokenException>(() => _service.RefreshTokenAsync(request));
        // }

        [Fact]
        public async Task GetProfileAsync_ReturnsProfile_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User 
            { 
                Id = userId,
                Username = "testuser",
                Email = "test@example.com"
            };
            var profileResponse = new ProfileResponseDto 
            { 
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<ProfileResponseDto>(user))
                .Returns(profileResponse);

            // Act
            var result = await _service.GetProfileAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetProfileAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _service.GetProfileAsync(userId));
        }

        [Fact]
        public async Task UpdateProfileAsync_ReturnsUpdatedProfile_WhenSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateProfileRequestDto 
            { 
                Bio = "updated user bio",
                Avatar = "updated user avatar"
            };
            var user = new User 
            { 
                Id = userId,
                Username = "olduser",
                Email = "old@example.com"
            };
            var updatedUser = new User 
            { 
                Id = userId,
                Username = request.Bio,
                Email = request.Avatar
            };
            var profileResponse = new ProfileResponseDto 
            { 
                Id = userId,
                Username = request.Bio,
                Email = request.Avatar
            };

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(updatedUser);
            _mockMapper.Setup(m => m.Map<ProfileResponseDto>(updatedUser))
                .Returns(profileResponse);

            // Act
            var result = await _service.UpdateProfileAsync(userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Bio, result.Username);
            Assert.Equal(request.Avatar, result.Email);
        }

        [Fact]
        public async Task UpdateProfileAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UpdateProfileRequestDto();

            _mockRepo.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _service.UpdateProfileAsync(userId, request);

            // Assert
            Assert.Null(result);
        }
    }
}