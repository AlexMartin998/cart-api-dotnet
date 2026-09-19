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
