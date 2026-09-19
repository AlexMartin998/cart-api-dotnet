using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Features.Catalog.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartAPI.Features.Catalog.Categories.Infrastructure.Persistence.Configurations;


internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", CatalogSchema.Name);
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(Category.NameMaxLength).IsRequired();
        builder.HasIndex(c => c.Name).IsUnique();
    }
}
