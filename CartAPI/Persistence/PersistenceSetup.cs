using CartAPI.Shared.Application;
using CartAPI.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CartAPI.Persistence;

public static class PersistenceSetup
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Configure(o => o.ConnectionString = configuration.GetConnectionString("Default") ?? string.Empty)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((sp, options) => options.UseSqlServer(
            sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString,
            sql => sql.EnableRetryOnFailure()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<ITransactionRunner, TransactionRunner>();
        return services;
    }

    public static async Task PrepareDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        if (services.GetRequiredService<IOptions<DatabaseOptions>>().Value.MigrateOnStartup)
            await services.GetRequiredService<AppDbContext>().Database.MigrateAsync(app.Lifetime.ApplicationStopping);

        foreach (var seeder in services.GetServices<IDataSeeder>().OrderBy(s => s.Order))
            await seeder.SeedAsync(app.Lifetime.ApplicationStopping);
    }
}
