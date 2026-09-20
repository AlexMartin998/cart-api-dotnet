using CartAPI.Features.Ordering.Orders.Domain;
using CartAPI.Persistence;

namespace CartAPI.Features.Ordering.Orders.Infrastructure.Persistence;


internal sealed class OrderRepository(AppDbContext db) : IOrderRepository
{
    public void Add(Order order) => db.Set<Order>().Add(order);
}
