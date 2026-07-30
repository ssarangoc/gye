using System.ComponentModel.DataAnnotations;

namespace Inventory.Domain.Entities;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "User";
}
