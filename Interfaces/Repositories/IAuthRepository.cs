using BlogApp.Models.Entities;

namespace BlogApp.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    
    Task<User> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
}