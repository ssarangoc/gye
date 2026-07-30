using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    void Remove(Product product);
    Task SaveChangesAsync();
}
