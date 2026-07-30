using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly InventoryDbContext _context;

    public SupplierRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers.FindAsync(id);
    }

    public async Task<Supplier?> GetByNameAsync(string name)
    {
        return await _context.Suppliers.FirstOrDefaultAsync(s => s.Name == name);
    }

    public void RemoveProductSuppliers(IEnumerable<ProductSupplier> productSuppliers)
    {
        _context.ProductSuppliers.RemoveRange(productSuppliers);
    }
}
