using CartAPI.Features.Accounts.Auth.Domain;
using CartAPI.Features.Accounts.Auth.Domain.ValueObjects;
using CartAPI.Features.Accounts.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartAPI.Features.Accounts.Auth.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", AccountsSchema.Name,
            t => t.HasCheckConstraint("CK_Users_Role", $"[Role] IN ('{Roles.Admin}', '{Roles.Customer}')"));
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).HasMaxLength(Email.MaxLength).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FullName).HasMaxLength(User.FullNameMaxLength).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Role).HasMaxLength(Roles.MaxLength).IsRequired();
    }
}
