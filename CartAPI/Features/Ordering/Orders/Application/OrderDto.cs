using CartAPI.Features.Ordering.Orders.Domain;

namespace CartAPI.Features.Ordering.Orders.Application;


public sealed record OrderDto(
    int Id, DateTime PlacedAt, decimal Subtotal, decimal Discount, decimal Total, IReadOnlyList<OrderItemDto> Items)
{
    
    public static OrderDto From(Order order) => new(
        order.Id, order.PlacedAt, order.Subtotal, order.Discount, order.Total,
        [
            .. order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductCode, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal))
        ]
    );
}

public sealed record OrderItemDto(int ProductId, string ProductCode, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
