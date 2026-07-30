using System.ComponentModel.DataAnnotations;

namespace Inventory.Application.DTOs;

public class ProductRequest
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    public List<ProductSupplierRequest> Suppliers { get; set; } = [];
}
