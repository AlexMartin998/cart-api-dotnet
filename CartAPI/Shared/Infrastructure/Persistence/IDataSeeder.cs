namespace CartAPI.Shared.Infrastructure.Persistence;

public interface IDataSeeder
{
    int Order { get; }

    Task SeedAsync(CancellationToken ct);
}
