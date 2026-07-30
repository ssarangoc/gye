using System.ComponentModel.DataAnnotations;

namespace Inventory.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProductSupplier> ProductSuppliers { get; set; } = [];
}
