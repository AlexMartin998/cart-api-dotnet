using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Features.Ordering.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartAPI.Features.Ordering.Carts.Infrastructure.Persistence.Configurations;


internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts", OrderingSchema.Name);
        builder.HasKey(c => c.Id);
        
        builder.HasIndex(c => c.UserId).IsUnique();


        builder.HasMany(c => c.Items).WithOne().HasForeignKey(i => i.CartId).OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
