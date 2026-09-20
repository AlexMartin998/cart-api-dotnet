using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Features.Ordering.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CartAPI.Features.Ordering.Carts.Infrastructure.Persistence.Configurations;


internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems", OrderingSchema.Name, t => t.HasCheckConstraint("CK_CartItems_Quantity", "[Quantity] > 0"));
        builder.HasKey(i => i.Id);

        builder.HasIndex(i => new { i.CartId, i.ProductId }).IsUnique();
    }
}
