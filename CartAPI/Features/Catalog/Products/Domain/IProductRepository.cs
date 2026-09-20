using CartAPI.Features.Catalog.Products.Domain.ValueObjects;

namespace CartAPI.Features.Catalog.Products.Domain;


public interface IProductRepository
{
    Task<bool> TryReserveStockAsync(int productId, int quantity, CancellationToken ct);

    Task<Product?> FindActiveAsync(int productId, CancellationToken ct);

    Task<bool> CodeExistsAsync(ProductCode code, CancellationToken ct);

    void Add(Product product);

}
