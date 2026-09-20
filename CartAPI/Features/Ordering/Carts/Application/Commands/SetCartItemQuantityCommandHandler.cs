using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Shared.Application;

namespace CartAPI.Features.Ordering.Carts.Application.Commands;


public sealed record SetCartItemQuantityCommand(int UserId, int ProductId, int Quantity);


public sealed class SetCartItemQuantityCommandHandler(
    ICartRepository carts, ICatalogStock catalog, CartPricer pricer, IUnitOfWork unitOfWork, TimeProvider clock)
{
    public async Task<CartDto> HandleAsync(SetCartItemQuantityCommand command, CancellationToken ct)
    {

        var cart = await carts.FindByUserAsync(command.UserId, ct) ?? throw CartErrors.LineNotFound(command.ProductId);
        if (cart.QuantityOf(command.ProductId) == 0)
            throw CartErrors.LineNotFound(command.ProductId);

        await CartAvailability.EnsureAsync(catalog, command.ProductId, command.Quantity, ct);

        cart.SetQuantity(command.ProductId, command.Quantity, clock.GetUtcNow().UtcDateTime);

        await unitOfWork.SaveChangesAsync(ct);

        return await pricer.PriceAsync(cart, ct);
    }
}
