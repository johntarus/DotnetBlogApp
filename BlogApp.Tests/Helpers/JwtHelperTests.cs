// File: JwtHelperTests.cs

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogApp.Core.Common.Helpers;
using BlogApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogApp.Tests.Helpers
{
    public class JwtHelperTests
    {
        private readonly IConfiguration _config;

        public JwtHelperTests()
        {
            var inMemorySettings = new[]
            {
                new KeyValuePair<string, string>("Jwt:Key", "test-secret-key-which-seems-to-be-32-bit-long-1234567890"),
                new KeyValuePair<string, string>("Jwt:Issuer", "BlogApp"),
                new KeyValuePair<string, string>("Jwt:Audience", "BlogAppUsers")
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public void GenerateAccessToken_Returns_ValidToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Role = "User",
                Username = null
            };

            // Act
            var token = JwtHelper.GenerateAccessToken(user, _config);

            // Assert
            Assert.NotNull(token);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal("BlogApp", jwtToken.Issuer);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Email && c.Value == user.Email);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == user.Role);
        }

        [Fact]
        public void GenerateRefreshToken_Returns_NonEmptyBase64String()
        {
            // Act
            var token = JwtHelper.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));

            var bytes = Convert.FromBase64String(token);
            Assert.Equal(32, bytes.Length);
        }

        [Fact]
        public void GetPrincipleFromExpiredToken_ReturnsClaimsPrincipal()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Role = "Admin",
                Username = null
            };

            // Create an expired token manually
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30), // already expired
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var expiredToken = tokenHandler.WriteToken(securityToken);

            // Act
            var principal = JwtHelper.GetPrincipleFromExpiredToken(expiredToken, _config);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal(user.Email, principal.FindFirst(ClaimTypes.Email)?.Value);
            Assert.Equal(user.Id.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal(user.Role, principal.FindFirst(ClaimTypes.Role)?.Value);
        }

        [Fact]
        public void GetPrincipleFromExpiredToken_Throws_OnInvalidToken()
        {
            // Arrange
            var invalidToken = "not-a-real-token";

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
                JwtHelper.GetPrincipleFromExpiredToken(invalidToken, _config));
        }
    }
}
