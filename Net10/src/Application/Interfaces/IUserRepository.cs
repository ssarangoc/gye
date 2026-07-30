using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsAsync(string username, string email);
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
