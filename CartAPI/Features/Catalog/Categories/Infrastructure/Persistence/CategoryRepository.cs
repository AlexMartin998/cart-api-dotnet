using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Persistence;

namespace CartAPI.Features.Catalog.Categories.Infrastructure.Persistence;


internal sealed class CategoryRepository(AppDbContext db) : ICategoryRepository
{

    public async Task<Category?> FindAsync(int categoryId, CancellationToken ct) =>
        await db.Set<Category>().FindAsync([categoryId], ct);

}
