using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Features.Catalog.Products.Domain.ValueObjects;
using CartAPI.Persistence;
using CartAPI.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Seeding;



internal sealed class ProductSeeder(AppDbContext db, TimeProvider clock) : IDataSeeder
{
    public int Order => 20;

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await db.Set<Product>().AnyAsync(ct))
            return;


        var categories = await db.Set<Category>().ToDictionaryAsync(c => c.Name, c => c.Id, ct);
        var now = clock.GetUtcNow().UtcDateTime;


        Product P(string code, string name, decimal price, int stock, string category, string description) =>
            Product.Create(ProductCode.Create(code), name, description, price, stock, categories[category], now);

        db.Set<Product>().AddRange(
            P("SKU-001", "Auriculares inalámbricos", 59.90m, 25, "Electrónica", "Bluetooth 5.3, 30 h de batería"),
            P("SKU-002", "Teclado mecánico", 89.50m, 12, "Electrónica", "Switches rojos, retroiluminado"),
            P("SKU-003", "Ratón ergonómico", 34.00m, 40, "Electrónica", "Vertical, 6 botones"),
            P("SKU-004", "Monitor 27 pulgadas", 249.99m, 2, "Electrónica", "QHD 165 Hz, stock bajo"),
            P("SKU-005", "Cafetera de goteo", 45.25m, 18, "Hogar", "1,5 L, jarra de cristal"),
            P("SKU-006", "Juego de sábanas", 39.90m, 30, "Hogar", "Algodón 200 hilos, cama de 150"),
            P("SKU-007", "Lámpara de escritorio", 27.50m, 22, "Hogar", "LED regulable, brazo articulado"),
            P("SKU-008", "Camiseta básica", 15.00m, 50, "Ropa", "Algodón orgánico, unisex"),
            P("SKU-009", "Sudadera con capucha", 42.75m, 20, "Ropa", "Interior afelpado"),
            P("SKU-010", "Pantalón vaquero", 55.00m, 16, "Ropa", "Corte recto, tejido elástico"),
            P("SKU-011", "Zapatillas de running", 79.99m, 1, "Deportes", "Amortiguación ligera, última unidad"),
            P("SKU-012", "Esterilla de yoga", 21.40m, 35, "Deportes", "6 mm, antideslizante"),
            P("SKU-013", "Mancuernas ajustables", 129.00m, 8, "Deportes", "Par, de 2 a 24 kg"),
            P("SKU-014", "Botella térmica", 18.60m, 45, "Deportes", "750 ml, acero inoxidable"),
            P("SKU-015", "Mochila de senderismo", 64.30m, 14, "Deportes", "30 L, impermeable"));

        await db.SaveChangesAsync(ct);
    }
}
