namespace CartAPI.Features.Catalog.Products.Domain;


public interface IProductRepository
{
    Task<bool> TryReserveStockAsync(int productId, int quantity, CancellationToken ct);
}
