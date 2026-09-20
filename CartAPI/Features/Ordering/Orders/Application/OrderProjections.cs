using CartAPI.Features.Ordering.Orders.Domain;
using CartAPI.Persistence;

namespace CartAPI.Features.Ordering.Orders.Application;


internal static class OrderProjections
{
    public static IQueryable<Order> OrdersOf(this AppDbContext db, int userId) =>
        db.Set<Order>().Where(o => o.UserId == userId);
}
