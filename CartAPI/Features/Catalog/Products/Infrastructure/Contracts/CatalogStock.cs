using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Contracts;


internal sealed class CatalogStock(AppDbContext db): ICatalogStock
{

    public async Task<IReadOnlyDictionary<int, ProductSnapshot>> PeekAsync(IReadOnlyCollection<int> productIds, CancellationToken ct)
    {
        if (productIds.Count == 0)
            return new Dictionary<int, ProductSnapshot>();
    
        return await db.Set<Product>().AsNoTracking()
            .Where(p => p.IsActive && productIds.Contains(p.Id))
            .Select(p => new ProductSnapshot(p.Id, p.Code, p.Name, p.Price, p.Stock))
            .ToDictionaryAsync(p => p.ProductId, ct);
    }
    
}
