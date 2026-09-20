using CartAPI.Features.Catalog.Products.Domain;

namespace CartAPI.Features.Catalog.Products.Application;


public sealed record ProductDto(
    int Id,
    string Code,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName
);


internal static class ProductDtoMapping
{
    public static ProductDto ToDto(this Product product, string categoryName) => new(
        product.Id, product.Code, product.Name, product.Description, product.Price, product.Stock, product.CategoryId, categoryName);
}
