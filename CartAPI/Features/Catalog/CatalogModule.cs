using CartAPI.Features.Catalog.Categories.Infrastructure.Persistence.Configurations;
using CartAPI.Features.Catalog.Categories.Infrastructure.Seeding;
using CartAPI.Features.Catalog.Categories.Application.Queries;
using CartAPI.Features.Catalog.Categories.Infrastructure.Http;
using CartAPI.Features.Catalog.Products.Application.Queries;
using CartAPI.Features.Catalog.Products.Infrastructure.Http;
using CartAPI.Features.Catalog.Products.Infrastructure.Persistence.Configurations;
using CartAPI.Features.Catalog.Products.Infrastructure.Seeding;
using CartAPI.Shared.Infrastructure.Persistence;
using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Catalog.Products.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog;


// DI ---
public static class CatalogModule
{
    
    public static IServiceCollection AddCatalogModule(this IServiceCollection services)
    {
        services.AddScoped<SearchProductsQueryHandler>();
        services.AddScoped<GetProductQueryHandler>();
        services.AddScoped<ListCategoriesQueryHandler>();

        // DI: other bounded context ---
        services.AddScoped<ICatalogStock, CatalogStock>();

        services.AddScoped<IDataSeeder, CategorySeeder>();
        services.AddScoped<IDataSeeder, ProductSeeder>();
        return services;
    }

    public static void ConfigureModel(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new ProductConfiguration());
    }


    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app) =>
        app.MapProductEndpoints().MapCategoryEndpoints();

}
