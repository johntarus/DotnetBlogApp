using System.Security.Claims;
using BlogApp.Data;
using BlogApp.Helpers;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _config;
    
    public UsersController(DatabaseContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already exists");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordHelper.HashPassword(request.Password),
        };

        _context.Add((user));
        await _context.SaveChangesAsync();
        return Ok(new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Token = JwtHelper.GenerateToken(user, _config)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail ||
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
            Token = JwtHelper.GenerateToken(user, _config)
        });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileResponseDto>> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Users.FindAsync(userId);
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
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();
        if(ModelState.IsValid == false)
            return BadRequest(ModelState);
        if(request.Bio != null)
            user.Bio = request.Bio;
        if(request.Avatar != null)
            user.Avatar = request.Avatar;
        user.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        
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