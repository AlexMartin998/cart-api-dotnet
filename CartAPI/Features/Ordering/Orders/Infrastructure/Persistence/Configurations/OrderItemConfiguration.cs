using CartAPI.Features.Ordering.Orders.Domain;
using CartAPI.Features.Ordering.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartAPI.Features.Ordering.Orders.Infrastructure.Persistence.Configurations;


internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", OrderingSchema.Name, t => t.HasCheckConstraint("CK_OrderItems_Quantity", "[Quantity] > 0"));
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductCode).HasMaxLength(OrderItem.ProductCodeMaxLength).IsRequired();
        builder.Property(i => i.ProductName).HasMaxLength(OrderItem.ProductNameMaxLength).IsRequired();

        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Property(i => i.LineTotal).HasPrecision(18, 2);
    }
}
