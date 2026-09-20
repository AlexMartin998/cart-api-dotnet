namespace CartAPI.Features.Catalog.Contracts;


public interface ICatalogStock
{
    Task<IReadOnlyDictionary<int, ProductSnapshot>> PeekAsync(IReadOnlyCollection<int> productIds, CancellationToken ct);
    Task<StockReservation> TryReserveAsync(int productId, int quantity, CancellationToken ct);

}


// primitive, port between bounded contexts
public sealed record ProductSnapshot(int ProductId, string Code, string Name, decimal UnitPrice, int Stock);

public sealed record StockReservation(ProductSnapshot? Product, ReservationFailure Failure)
{
    public static StockReservation Reserved(ProductSnapshot product) => new(product, ReservationFailure.None);

    public static StockReservation Failed(ReservationFailure failure, ProductSnapshot? current = null) => new(current, failure);
}

public enum ReservationFailure
{
    None,
    ProductNotFound,
    InsufficientStock,
}
