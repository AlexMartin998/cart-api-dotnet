namespace CartAPI.Features.Ordering.Carts.Domain;


public interface ICartRepository
{
    Task<Cart?> FindByUserAsync(int userId, CancellationToken ct);

    void Add(Cart cart);
}
