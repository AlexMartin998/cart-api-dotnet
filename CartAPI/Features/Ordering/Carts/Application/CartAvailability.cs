using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Ordering.Shared.Domain;

namespace CartAPI.Features.Ordering.Carts.Application;


// service application ---
internal static class CartAvailability
{
    public static async Task EnsureAsync(ICatalogStock catalog, int productId, int resultingQuantity, CancellationToken ct)
    {
        var product = (await catalog.PeekAsync([productId], ct)).GetValueOrDefault(productId)
            ?? throw OrderingErrors.ProductNotFound(productId);

        if (resultingQuantity > product.Stock)
            throw OrderingErrors.InsufficientStock(product.Code, product.Stock);
    }
}
