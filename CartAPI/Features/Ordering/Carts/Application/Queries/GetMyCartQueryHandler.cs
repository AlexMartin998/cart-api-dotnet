using CartAPI.Features.Ordering.Carts.Domain;

namespace CartAPI.Features.Ordering.Carts.Application.Queries;

public sealed record GetMyCartQuery(int UserId);


public sealed class GetMyCartQueryHandler(ICartRepository carts, CartPricer pricer)
{
    public async Task<CartDto> HandleAsync(GetMyCartQuery query, CancellationToken ct) =>
        await pricer.PriceAsync(await carts.FindByUserAsync(query.UserId, ct), ct);
}
