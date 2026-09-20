using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Features.Ordering.Orders.Domain;
using CartAPI.Features.Ordering.Shared.Domain;
using CartAPI.Shared.Application;

namespace CartAPI.Features.Ordering.Orders.Application.Commands;



public sealed record PlaceOrderCommand(int UserId);

// transactional command handler
public sealed partial class PlaceOrderCommandHandler(
    ICartRepository carts,
    ICatalogStock catalog,
    IOrderRepository orders,
    OrderPricing pricing,
    ITransactionRunner transactions,
    TimeProvider clock,
    ILogger<PlaceOrderCommandHandler> logger)
{
    public async Task<OrderDto> HandleAsync(PlaceOrderCommand command, CancellationToken ct)
    {

        var order = await transactions.ExecuteAsync(async token =>
        {
            var cart = await carts.FindByUserAsync(command.UserId, token);
            if (cart is null || cart.IsEmpty)
                throw OrderErrors.CartEmpty();

            var reserved = new List<OrderLine>();

            foreach (var line in cart.Lines())
                reserved.Add(await ReserveAsync(line, token));

            var now = clock.GetUtcNow().UtcDateTime;
            var placed = Order.Place(command.UserId, reserved, pricing, now);
            orders.Add(placed);

            cart.Clear(now);
            return placed;
        }, ct);

        LogPlaced(order.Id, command.UserId, order.Total);
        return OrderDto.From(order);
    }


    private async Task<OrderLine> ReserveAsync(CartLine line, CancellationToken ct)
    {
        var reservation = await catalog.TryReserveAsync(line.ProductId, line.Quantity, ct);

        return reservation switch
        {
            { Failure: ReservationFailure.None, Product: { } p } =>
                new OrderLine(p.ProductId, p.Code, p.Name, p.UnitPrice, line.Quantity),
            { Failure: ReservationFailure.InsufficientStock, Product: { } p } =>
                throw OrderingErrors.InsufficientStock(p.Code, p.Stock),
            _ => throw OrderingErrors.ProductNotFound(line.ProductId),
        };
    }


    [LoggerMessage(Level = LogLevel.Information, Message = "Order {OrderId} placed by user {UserId} for {Total}")]
    private partial void LogPlaced(int orderId, int userId, decimal total);
}
