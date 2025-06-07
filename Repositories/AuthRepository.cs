using BlogApp.Data;
using BlogApp.Interfaces.Repositories;
using BlogApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories;

public class AuthRepository(DatabaseContext context) : IAuthRepository
{
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.ToLower().Trim();
        Console.WriteLine($"Searching for user with email: '{normalizedEmail}'");
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower().Trim() == normalizedEmail);
        Console.WriteLine(user != null ? "User found" : "User not found");
        return user;
    }


    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }
}