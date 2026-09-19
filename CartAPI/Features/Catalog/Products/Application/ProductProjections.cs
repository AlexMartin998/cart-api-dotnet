using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Persistence;

namespace CartAPI.Features.Catalog.Products.Application;


internal static class ProductProjections
{
    
    public static IQueryable<Product> ActiveProducts(this AppDbContext db) =>
        db.Set<Product>().Where(p => p.IsActive);


    // firter and ordering before projection to avoid loading unnecessary data
    public static IQueryable<ProductDto> ToDtos(this IQueryable<Product> products, AppDbContext db) =>
        products.Select(p => new ProductDto(
            p.Id, p.Code, p.Name, p.Description, p.Price, p.Stock, p.CategoryId,
            db.Set<Category>().Where(c => c.Id == p.CategoryId).Select(c => c.Name).First())
        );

}
