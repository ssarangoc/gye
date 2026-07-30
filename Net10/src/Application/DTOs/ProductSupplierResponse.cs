namespace Inventory.Application.DTOs;

public class ProductSupplierResponse
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
