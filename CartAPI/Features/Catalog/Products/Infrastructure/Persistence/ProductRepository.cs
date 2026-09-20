using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Persistence;


internal sealed class ProductRepository(AppDbContext db) : IProductRepository
{
    
    public async Task<bool> TryReserveStockAsync(int productId, int quantity, CancellationToken ct) =>
        await db.Set<Product>()
            .Where(p => p.Id == productId && p.IsActive && p.Stock >= quantity)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - quantity), ct) == 1;

}
