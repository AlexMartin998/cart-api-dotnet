using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Features.Catalog.Products.Domain.ValueObjects;
using CartAPI.Shared.Application;

namespace CartAPI.Features.Catalog.Products.Application.Commands;



public sealed record CreateProductCommand(
    string Code, string Name, string? Description, decimal Price, int Stock, int CategoryId
);


public sealed class CreateProductCommandHandler(
    IProductRepository products, ICategoryRepository categories, IUnitOfWork unitOfWork, TimeProvider clock)
{
    public async Task<ProductDto> HandleAsync(CreateProductCommand command, CancellationToken ct)
    {
        var code = ProductCode.Create(command.Code);
        var category = await categories.FindAsync(command.CategoryId, ct) ?? throw CategoryErrors.NotFound(command.CategoryId);

        if (await products.CodeExistsAsync(code, ct))
            throw ProductErrors.CodeTaken(code.Value);

        var product = Product.Create(code, command.Name, command.Description, command.Price, command.Stock, category.Id,
            clock.GetUtcNow().UtcDateTime);
        products.Add(product);
        await unitOfWork.SaveChangesAsync(ct);
        return product.ToDto(category.Name);
    }
}
