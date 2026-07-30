using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public ProductService(IProductRepository productRepository, ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<List<ProductResponse>> GetProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapProduct).ToList();
    }

    public async Task<ProductResponse?> GetProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product is null ? null : MapProduct(product);
    }

    public async Task<(ProductResponse? Product, string? ErrorMessage)> CreateProductAsync(ProductRequest request)
    {
        var suppliersResult = await BuildProductSuppliersAsync(request.Suppliers);
        if (suppliersResult.ErrorMessage is not null)
        {
            return (null, suppliersResult.ErrorMessage);
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProductSuppliers = suppliersResult.ProductSuppliers
        };

        UpdateProductSummary(product);

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        var createdProduct = await _productRepository.GetByIdAsync(product.Id);
        return (MapProduct(createdProduct!), null);
    }

    public async Task<(bool Success, bool NotFound, string? ErrorMessage)> UpdateProductAsync(int id, ProductRequest request)
    {
        if (request.Id != 0 && id != request.Id)
        {
            return (false, false, "El id de la ruta no coincide con el del cuerpo.");
        }

        var existing = await _productRepository.GetByIdAsync(id);
        if (existing is null)
        {
            return (false, true, null);
        }

        var suppliersResult = await BuildProductSuppliersAsync(request.Suppliers);
        if (suppliersResult.ErrorMessage is not null)
        {
            return (false, false, suppliersResult.ErrorMessage);
        }

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Category = request.Category;
        existing.UpdatedAt = DateTime.UtcNow;

        _supplierRepository.RemoveProductSuppliers(existing.ProductSuppliers);
        existing.ProductSuppliers.Clear();

        foreach (var productSupplier in suppliersResult.ProductSuppliers)
        {
            existing.ProductSuppliers.Add(productSupplier);
        }

        UpdateProductSummary(existing);

        await _productRepository.SaveChangesAsync();
        return (true, false, null);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return false;
        }

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync();
        return true;
    }

    private async Task<(List<ProductSupplier> ProductSuppliers, string? ErrorMessage)> BuildProductSuppliersAsync(List<ProductSupplierRequest> requests)
    {
        var productSuppliers = new List<ProductSupplier>();

        foreach (var request in requests)
        {
            Supplier? supplier = null;

            if (request.SupplierId.HasValue)
            {
                supplier = await _supplierRepository.GetByIdAsync(request.SupplierId.Value);
                if (supplier is null)
                {
                    return ([], $"No existe el proveedor con id {request.SupplierId.Value}.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.SupplierName))
            {
                var supplierName = request.SupplierName.Trim();
                supplier = await _supplierRepository.GetByNameAsync(supplierName);

                if (supplier is null)
                {
                    supplier = new Supplier
                    {
                        Name = supplierName
                    };
                }
            }
            else
            {
                return ([], "Cada detalle debe incluir SupplierId o SupplierName.");
            }

            productSuppliers.Add(new ProductSupplier
            {
                Supplier = supplier,
                Price = request.Price,
                Stock = request.Stock
            });
        }

        var duplicateSuppliers = productSuppliers
            .GroupBy(ps => ps.Supplier.Id != 0 ? $"id:{ps.Supplier.Id}" : $"name:{ps.Supplier.Name.ToUpperInvariant()}")
            .Any(group => group.Count() > 1);

        if (duplicateSuppliers)
        {
            return ([], "No se puede repetir el mismo proveedor dentro del mismo producto.");
        }

        return (productSuppliers, null);
    }

    private static ProductResponse MapProduct(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Suppliers = product.ProductSuppliers
                .OrderBy(ps => ps.Supplier.Name)
                .Select(ps => new ProductSupplierResponse
                {
                    SupplierId = ps.SupplierId,
                    SupplierName = ps.Supplier.Name,
                    Price = ps.Price,
                    Stock = ps.Stock
                })
                .ToList()
        };
    }

    private static void UpdateProductSummary(Product product)
    {
        if (product.ProductSuppliers.Count == 0)
        {
            product.Price = 0;
            product.Stock = 0;
            return;
        }

        product.Price = product.ProductSuppliers.Min(ps => ps.Price);
        product.Stock = product.ProductSuppliers.Sum(ps => ps.Stock);
    }
}
