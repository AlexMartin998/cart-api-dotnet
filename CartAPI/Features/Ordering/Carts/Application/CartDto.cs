namespace CartAPI.Features.Ordering.Carts.Application;


public sealed record CartDto(IReadOnlyList<CartItemDto> Items, int ItemCount, decimal Subtotal, decimal Discount, decimal Total)
{
    public static readonly CartDto Empty = new([], 0, 0m, 0m, 0m);
}


public sealed record CartItemDto(
    int ProductId, string? Code, string? Name, decimal UnitPrice, int Quantity, decimal LineTotal, int AvailableStock, string Status);


public static class CartItemStatus
{
    public const string Ok = "ok";
    public const string InsufficientStock = "insufficient_stock";
    public const string Unavailable = "unavailable";
}
