using CartAPI.Features.Ordering.Shared.Domain;

namespace CartAPI.Features.Ordering.Orders.Domain;


// aggregate root: Order manages OrderItem
public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public int Id { get; private set; }

    public int UserId { get; private set; }

    public DateTime PlacedAt { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal Discount { get; private set; }

    public decimal Total { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items;

    private Order() { }


    // ---------------
    public static Order Place(int userId, IReadOnlyList<OrderLine> lines, OrderPricing pricing, DateTime now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ArgumentNullException.ThrowIfNull(pricing);
        if (lines is null || lines.Count == 0)
            throw OrderErrors.NoLines();

        // no recalculate, just freeze the numbers at the moment of place
        var totals = pricing.For(lines.Select(l => l.LineTotal));
        var order = new Order
        {
            UserId = userId,
            PlacedAt = now,
            Subtotal = totals.Subtotal,
            Discount = totals.Discount,
            Total = totals.Total,
        };
        order._items.AddRange(lines.Select(l => new OrderItem(l)));
        return order;
    }
}
