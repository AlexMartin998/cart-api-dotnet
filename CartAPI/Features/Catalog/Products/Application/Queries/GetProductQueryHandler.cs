using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog.Products.Application.Queries;


public sealed record GetProductQuery(int ProductId);


public sealed class GetProductQueryHandler(AppDbContext db)
{
    
    public async Task<ProductDto> HandleAsync(GetProductQuery query, CancellationToken ct) =>
        await db.ActiveProducts().AsNoTracking()
            .Where(p => p.Id == query.ProductId)
            .ToDtos(db)
            .FirstOrDefaultAsync(ct)
        ?? throw ProductErrors.NotFound(query.ProductId);

}
