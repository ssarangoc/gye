using System.ComponentModel.DataAnnotations;

namespace Inventory.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(150)]
    public string? ContactName { get; set; }

    [StringLength(150)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    public ICollection<ProductSupplier> ProductSuppliers { get; set; } = [];
}
