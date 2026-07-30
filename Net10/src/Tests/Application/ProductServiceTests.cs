using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Moq;
using Xunit;

namespace Inventory.Tests.Application;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateProductAsync_WithExistingSupplierId_CalculatesPriceAndStockSummary()
    {
        var productRepository = new Mock<IProductRepository>();
        var supplierRepository = new Mock<ISupplierRepository>();

        var supplier = new Supplier { Id = 1, Name = "Acme" };
        supplierRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(supplier);

        Product? addedProduct = null;
        productRepository
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => addedProduct = p)
            .Returns(Task.CompletedTask);

        productRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(() => addedProduct);

        var service = new ProductService(productRepository.Object, supplierRepository.Object);

        var request = new ProductRequest
        {
            Name = "Producto de prueba",
            Category = "General",
            Suppliers =
            [
                new ProductSupplierRequest { SupplierId = 1, Price = 10m, Stock = 5 }
            ]
        };

        var (result, errorMessage) = await service.CreateProductAsync(request);

        Assert.Null(errorMessage);
        Assert.NotNull(result);
        Assert.Equal(10m, result!.Price);
        Assert.Equal(5, result.Stock);
        productRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_WithUnknownSupplierId_ReturnsErrorMessage()
    {
        var productRepository = new Mock<IProductRepository>();
        var supplierRepository = new Mock<ISupplierRepository>();

        supplierRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Supplier?)null);

        var service = new ProductService(productRepository.Object, supplierRepository.Object);

        var request = new ProductRequest
        {
            Name = "Producto de prueba",
            Category = "General",
            Suppliers =
            [
                new ProductSupplierRequest { SupplierId = 99, Price = 10m, Stock = 5 }
            ]
        };

        var (result, errorMessage) = await service.CreateProductAsync(request);

        Assert.Null(result);
        Assert.Equal("No existe el proveedor con id 99.", errorMessage);
        productRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        var productRepository = new Mock<IProductRepository>();
        var supplierRepository = new Mock<ISupplierRepository>();

        productRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var service = new ProductService(productRepository.Object, supplierRepository.Object);

        var success = await service.DeleteProductAsync(1);

        Assert.False(success);
        productRepository.Verify(r => r.Remove(It.IsAny<Product>()), Times.Never);
    }
}
