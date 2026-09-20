using CartAPI.Persistence;
using CartAPI.Shared.Application.Paging;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Ordering.Orders.Application.Queries;



public sealed record ListMyOrdersQuery(int UserId, PageRequest Page);


public sealed class ListMyOrdersQueryHandler(AppDbContext db)
{
    public Task<PagedResult<OrderSummaryDto>> HandleAsync(ListMyOrdersQuery query, CancellationToken ct) =>
        db.OrdersOf(query.UserId).AsNoTracking()
            .OrderByDescending(o => o.PlacedAt)
            .ThenByDescending(o => o.Id)
            .Select(o => new OrderSummaryDto(o.Id, o.PlacedAt, o.Items.Sum(i => i.Quantity), o.Subtotal, o.Discount, o.Total))
            .ToPagedResultAsync(query.Page, ct);
}
