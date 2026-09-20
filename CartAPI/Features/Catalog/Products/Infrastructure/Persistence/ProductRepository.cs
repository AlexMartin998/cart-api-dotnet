using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Features.Catalog.Products.Domain.ValueObjects;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Persistence;


internal sealed class ProductRepository(AppDbContext db) : IProductRepository
{
    
    public async Task<bool> TryReserveStockAsync(int productId, int quantity, CancellationToken ct) =>
        await db.Set<Product>()
            .Where(p => p.Id == productId && p.IsActive && p.Stock >= quantity)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - quantity), ct) == 1;

    public Task<Product?> FindActiveAsync(int productId, CancellationToken ct) =>
        db.Set<Product>().FirstOrDefaultAsync(p => p.Id == productId && p.IsActive, ct);

    public Task<bool> CodeExistsAsync(ProductCode code, CancellationToken ct) =>
        db.Set<Product>().AnyAsync(p => p.Code == code.Value, ct);

    public void Add(Product product) => db.Set<Product>().Add(product);

}
