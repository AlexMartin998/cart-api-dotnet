using CartAPI.Features.Ordering.Orders.Domain;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Ordering.Orders.Application.Queries;



public sealed record GetMyOrderQuery(int UserId, int OrderId);


public sealed class GetMyOrderQueryHandler(AppDbContext db)
{

    public async Task<OrderDto> HandleAsync(GetMyOrderQuery query, CancellationToken ct) =>
        await db.OrdersOf(query.UserId).AsNoTracking()
            .Where(o => o.Id == query.OrderId)
            .Select(o => new OrderDto(o.Id, o.PlacedAt, o.Subtotal, o.Discount, o.Total,
                o.Items.OrderBy(i => i.Id)
                    .Select(i => new OrderItemDto(i.ProductId, i.ProductCode, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal))
                    .ToList())
            )
            .FirstOrDefaultAsync(ct)
        ?? throw OrderErrors.NotFound(query.OrderId);
}
