namespace CartAPI.Features.Catalog.Contracts;


public interface ICatalogStock
{
    Task<IReadOnlyDictionary<int, ProductSnapshot>> PeekAsync(IReadOnlyCollection<int> productIds, CancellationToken ct);
}


// primitive, port between bounded contexts
public sealed record ProductSnapshot(int ProductId, string Code, string Name, decimal UnitPrice, int Stock);

