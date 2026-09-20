using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Shared.Application;

namespace CartAPI.Features.Ordering.Carts.Application.Commands;

public sealed record RemoveCartItemCommand(int UserId, int ProductId);


public sealed class RemoveCartItemCommandHandler(ICartRepository carts, CartPricer pricer, IUnitOfWork unitOfWork, TimeProvider clock)
{
    public async Task<CartDto> HandleAsync(RemoveCartItemCommand command, CancellationToken ct)
    {
        var cart = await carts.FindByUserAsync(command.UserId, ct) ?? throw CartErrors.LineNotFound(command.ProductId);

        cart.Remove(command.ProductId, clock.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(ct);
        return await pricer.PriceAsync(cart, ct);
    }
}
