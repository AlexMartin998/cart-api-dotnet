using CartAPI.Features.Catalog.Categories.Infrastructure.Persistence.Configurations;
using CartAPI.Features.Catalog.Categories.Infrastructure.Seeding;
using CartAPI.Features.Catalog.Products.Infrastructure.Persistence.Configurations;
using CartAPI.Features.Catalog.Products.Infrastructure.Seeding;
using CartAPI.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog;


// DI ---
public static class CatalogModule
{
    
    public static IServiceCollection AddCatalogModule(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, CategorySeeder>();
        services.AddScoped<IDataSeeder, ProductSeeder>();
        return services;
    }

    public static void ConfigureModel(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new ProductConfiguration());
    }

}
