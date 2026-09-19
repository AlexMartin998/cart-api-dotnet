using CartAPI.Features.Accounts;
using CartAPI.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        AccountsModule.ConfigureModel(modelBuilder);
    }
}
