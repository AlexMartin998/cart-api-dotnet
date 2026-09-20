using CartAPI.Features.Ordering.Orders.Domain;
using CartAPI.Features.Ordering.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartAPI.Features.Ordering.Orders.Infrastructure.Persistence.Configurations;


internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", OrderingSchema.Name, t => t.HasCheckConstraint("CK_Orders_Total", "[Total] >= 0"));
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Subtotal).HasPrecision(18, 2);
        builder.Property(o => o.Discount).HasPrecision(18, 2);
        builder.Property(o => o.Total).HasPrecision(18, 2);


        builder.HasIndex(o => new { o.UserId, o.PlacedAt });

        builder.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
