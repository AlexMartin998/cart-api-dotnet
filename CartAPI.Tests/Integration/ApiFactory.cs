using CartAPI.Persistence;
using CartAPI.Shared.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CartAPI.Tests.Integration;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    // An in-memory SQLite database lives as long as its connection.
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Default", "sqlite-in-memory");
        builder.UseSetting("Database:MigrateOnStartup", "false");
        builder.UseSetting("Jwt:Key", "integration-tests-key-with-at-least-32-chars");

        builder.ConfigureServices(services =>
        {
            // Since .NET 9 AddDbContext also registers this, carrying UseSqlServer.
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

            services.AddScoped<IDataSeeder, CreateSchema>();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }

    private sealed class CreateSchema(AppDbContext db) : IDataSeeder
    {
        public int Order => int.MinValue;

        public Task SeedAsync(CancellationToken ct) => db.Database.EnsureCreatedAsync(ct);
    }
}
