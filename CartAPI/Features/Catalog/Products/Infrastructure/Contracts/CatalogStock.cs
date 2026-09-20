using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Contracts;


internal sealed class CatalogStock(IProductRepository products, AppDbContext db) : ICatalogStock
{

    public async Task<IReadOnlyDictionary<int, ProductSnapshot>> PeekAsync(IReadOnlyCollection<int> productIds, CancellationToken ct)
    {
        if (productIds.Count == 0)
            return new Dictionary<int, ProductSnapshot>();
    
        return await Snapshots(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.ProductId, ct);
    }

    public async Task<StockReservation> TryReserveAsync(int productId, int quantity, CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        var reserved = await products.TryReserveStockAsync(productId, quantity, ct);

        var current = await Snapshots(p => p.Id == productId).FirstOrDefaultAsync(ct);
        if (current is null)
            return StockReservation.Failed(ReservationFailure.ProductNotFound);

        return reserved
            ? StockReservation.Reserved(current)
            : StockReservation.Failed(ReservationFailure.InsufficientStock, current);
    }

    private IQueryable<ProductSnapshot> Snapshots(Expression<Func<Product, bool>> filter) =>
        db.Set<Product>().AsNoTracking()
            .Where(p => p.IsActive)
            .Where(filter)
            .Select(p => new ProductSnapshot(p.Id, p.Code, p.Name, p.Price, p.Stock));

}
