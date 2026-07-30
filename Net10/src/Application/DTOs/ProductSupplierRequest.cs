using System.ComponentModel.DataAnnotations;

namespace Inventory.Application.DTOs;

public class ProductSupplierRequest
{
    public int? SupplierId { get; set; }

    [StringLength(150)]
    public string? SupplierName { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}
