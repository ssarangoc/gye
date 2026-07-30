using Inventory.Domain.Entities;

namespace Inventory.Application.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);
    Task<Supplier?> GetByNameAsync(string name);
    void RemoveProductSuppliers(IEnumerable<ProductSupplier> productSuppliers);
}
