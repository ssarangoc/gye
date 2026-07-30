using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<ProductSupplier> ProductSuppliers => Set<ProductSupplier>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductSupplier>()
            .HasKey(ps => new { ps.ProductId, ps.SupplierId });

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSuppliers)
            .HasForeignKey(ps => ps.ProductId);

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Supplier)
            .WithMany(s => s.ProductSuppliers)
            .HasForeignKey(ps => ps.SupplierId);

        modelBuilder.Entity<Supplier>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "$2a$11$L1UMs1G0Y3rF/0E08yiR5eK5xQFZ2RM3NhYdQkYJ5Pj1mYgh/4nu",
                Email = "admin@inventory.com",
                Role = "Admin"
            });
    }
}
