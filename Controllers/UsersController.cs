using BlogApp.Data;
using BlogApp.Helpers;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;
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
}