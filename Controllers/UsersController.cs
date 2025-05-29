using System.Security.Claims;
using BlogApp.Data;
using BlogApp.Helpers;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using BlogApp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(DatabaseContext context, IAuthService authService, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var user = await authService.RegisterAsync(request);
        if(ModelState.IsValid == false) return BadRequest(ModelState);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail ||
                                                                 u.Email == request.UsernameOrEmail);
        if(string.IsNullOrWhiteSpace(request.UsernameOrEmail) && string.IsNullOrWhiteSpace(request.UsernameOrEmail))
            return BadRequest("Username or Email is required");
        if(string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Password is required");
        if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid Credentials");
        }
        return Ok(new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt,
            Token = JwtHelper.GenerateToken(user, config)
        });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileResponseDto>> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await context.Users.FindAsync(userId);
        if (user == null) return NotFound();
        return Ok(new ProfileResponseDto()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        });
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task<ActionResult<UpdateProfileRequestDto>> UpdateProfile(UpdateProfileRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await context.Users.FindAsync(userId);
        if (user == null) return NotFound();
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        if(request.Bio != null)
            user.Bio = request.Bio;
        if(request.Avatar != null)
            user.Avatar = request.Avatar;
        user.UpdatedAt = DateTime.Now;
        await context.SaveChangesAsync();
        
        return Ok(new ProfileResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            Avatar = user.Avatar,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        });
    }
}