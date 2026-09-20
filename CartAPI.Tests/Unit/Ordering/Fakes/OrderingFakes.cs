using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Shared.Application;

namespace CartAPI.Tests.Unit.Ordering.Fakes;


internal sealed class FakeCatalog(params ProductSnapshot[] products) : ICatalogStock
{
    private readonly Dictionary<int, ProductSnapshot> _products = products.ToDictionary(p => p.ProductId);

    public Task<IReadOnlyDictionary<int, ProductSnapshot>> PeekAsync(IReadOnlyCollection<int> productIds, CancellationToken ct) =>
        Task.FromResult<IReadOnlyDictionary<int, ProductSnapshot>>(
            _products.Values.Where(p => productIds.Contains(p.ProductId)).ToDictionary(p => p.ProductId));

    public Task<StockReservation> TryReserveAsync(int productId, int quantity, CancellationToken ct)
    {
        if (!_products.TryGetValue(productId, out var product))
            return Task.FromResult(StockReservation.Failed(ReservationFailure.ProductNotFound));
        if (product.Stock < quantity)
            return Task.FromResult(StockReservation.Failed(ReservationFailure.InsufficientStock, product));

        _products[productId] = product with { Stock = product.Stock - quantity };
        return Task.FromResult(StockReservation.Reserved(product));
    }
}


internal sealed class FakeCarts : ICartRepository
{
    public Cart? Stored { get; private set; }

    public Task<Cart?> FindByUserAsync(int userId, CancellationToken ct) =>
        Task.FromResult(Stored?.UserId == userId ? Stored : null);

    public void Add(Cart cart) => Stored = cart;
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int Saves { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saves++;
        return Task.FromResult(1);
    }
}
