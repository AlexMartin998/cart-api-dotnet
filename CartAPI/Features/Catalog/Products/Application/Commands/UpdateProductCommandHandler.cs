using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Shared.Application;

namespace CartAPI.Features.Catalog.Products.Application.Commands;



public sealed record UpdateProductCommand(int ProductId, string Name, string? Description, decimal Price, int Stock, int CategoryId);


public sealed class UpdateProductCommandHandler(
    IProductRepository products, ICategoryRepository categories, IUnitOfWork unitOfWork, TimeProvider clock)
{
    public async Task<ProductDto> HandleAsync(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await products.FindActiveAsync(command.ProductId, ct) ?? throw ProductErrors.NotFound(command.ProductId);
        var category = await categories.FindAsync(command.CategoryId, ct) ?? throw CategoryErrors.NotFound(command.CategoryId);


        product.Update(command.Name, command.Description, command.Price, command.Stock, category.Id, clock.GetUtcNow().UtcDateTime);

        await unitOfWork.SaveChangesAsync(ct);

        return product.ToDto(category.Name);
    }
}
