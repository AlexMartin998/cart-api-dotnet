using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Shared.Application;

namespace CartAPI.Features.Catalog.Products.Application.Commands;



public sealed record DeactivateProductCommand(int ProductId);


public sealed class DeactivateProductCommandHandler(IProductRepository products, IUnitOfWork unitOfWork, TimeProvider clock)
{

    public async Task HandleAsync(DeactivateProductCommand command, CancellationToken ct)
    {
        var product = await products.FindActiveAsync(command.ProductId, ct) ?? throw ProductErrors.NotFound(command.ProductId);

        product.Deactivate(clock.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(ct);
    }

}
