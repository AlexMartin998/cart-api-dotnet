using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Features.Catalog.Products.Domain.ValueObjects;
using CartAPI.Features.Catalog.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Persistence.Configurations;


internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", CatalogSchema.Name, t => t.HasCheckConstraint("CK_Products_Stock", "[Stock] >= 0"));
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code).HasMaxLength(ProductCode.MaxLength).IsRequired();
        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Name).HasMaxLength(Product.NameMaxLength).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(Product.DescriptionMaxLength);

        builder.Property(p => p.Price).HasPrecision(18, 2);

        
        builder.HasOne<Category>().WithMany().HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(p => new { p.IsActive, p.CategoryId });
    }
}
