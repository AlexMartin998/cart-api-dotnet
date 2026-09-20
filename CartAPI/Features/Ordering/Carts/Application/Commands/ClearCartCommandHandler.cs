using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Shared.Application;

namespace CartAPI.Features.Ordering.Carts.Application.Commands;


public sealed record ClearCartCommand(int UserId);


public sealed class ClearCartCommandHandler(ICartRepository carts, IUnitOfWork unitOfWork, TimeProvider clock)
{
    public async Task HandleAsync(ClearCartCommand command, CancellationToken ct)
    {
        var cart = await carts.FindByUserAsync(command.UserId, ct);
        if (cart is null || cart.IsEmpty)
            return;

        cart.Clear(clock.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
