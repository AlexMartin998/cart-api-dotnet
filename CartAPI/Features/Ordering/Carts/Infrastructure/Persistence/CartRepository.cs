using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Ordering.Carts.Infrastructure.Persistence;


internal sealed class CartRepository(AppDbContext db) : ICartRepository
{

    public Task<Cart?> FindByUserAsync(int userId, CancellationToken ct) =>
        db.Set<Cart>().Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId, ct);

    
    public void Add(Cart cart) => db.Set<Cart>().Add(cart);

}
