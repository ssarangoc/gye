using System.ComponentModel.DataAnnotations;

namespace Inventory.Domain.Entities;

public class ProductSupplier
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}
