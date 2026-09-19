using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Persistence;
using CartAPI.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog.Categories.Infrastructure.Seeding;


internal sealed class CategorySeeder(AppDbContext db) : IDataSeeder
{
    public static readonly string[] Names = ["Electrónica", "Hogar", "Ropa", "Deportes"];

    public int Order => 10;

    
    public async Task SeedAsync(CancellationToken ct)
    {
        if (await db.Set<Category>().AnyAsync(ct))
            return;

        db.Set<Category>().AddRange(Names.Select(Category.Create));
        await db.SaveChangesAsync(ct);
    }
}
