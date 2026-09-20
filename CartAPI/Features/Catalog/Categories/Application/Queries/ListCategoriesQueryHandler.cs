using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog.Categories.Application.Queries;


public sealed class ListCategoriesQueryHandler(AppDbContext db)
{

    public async Task<IReadOnlyCollection<CategoryDto>> HandleAsync(CancellationToken ct) => 
        await db.Set<Category>().AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync(ct);

}
