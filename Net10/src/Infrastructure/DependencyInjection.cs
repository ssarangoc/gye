using Inventory.Application.Interfaces;
using Inventory.Application.Services;
using Inventory.Infrastructure.Data;
using Inventory.Infrastructure.Repositories;
using Inventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")!));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        services.AddScoped<ProductService>();
        services.AddScoped<AuthService>();

        return services;
    }
}
