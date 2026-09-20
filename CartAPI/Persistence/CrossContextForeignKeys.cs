using CartAPI.Features.Accounts.Auth.Domain;
using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Features.Ordering.Carts.Domain;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Persistence;


// fk in db, without navigation in C#
internal static class CrossContextForeignKeys
{
    public static void Configure(ModelBuilder builder)
    {
        // fk between contexts
        builder.Entity<Cart>().HasOne<User>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CartItem>().HasOne<Product>().WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
