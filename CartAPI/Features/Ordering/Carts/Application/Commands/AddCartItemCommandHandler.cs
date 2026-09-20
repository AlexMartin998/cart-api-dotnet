using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Shared.Application;

namespace CartAPI.Features.Ordering.Carts.Application.Commands;


public sealed record AddCartItemCommand(int UserId, int ProductId, int Quantity);


public sealed class AddCartItemCommandHandler(
    ICartRepository carts, ICatalogStock catalog, CartPricer pricer, IUnitOfWork unitOfWork, TimeProvider clock)
{
    public async Task<CartDto> HandleAsync(AddCartItemCommand command, CancellationToken ct)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var cart = await carts.FindByUserAsync(command.UserId, ct);
        if (cart is null)
        {
            cart = Cart.Open(command.UserId, now);
            carts.Add(cart);
        }


        var resulting = cart.QuantityOf(command.ProductId) + command.Quantity;
        await CartAvailability.EnsureAsync(catalog, command.ProductId, resulting, ct);

        cart.AddOrIncrease(command.ProductId, command.Quantity, now);

        await unitOfWork.SaveChangesAsync(ct); // EF

        return await pricer.PriceAsync(cart, ct);
    }
}
