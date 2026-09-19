using CartAPI.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Shared.Infrastructure.Persistence;

internal sealed class TransactionRunner(DbContext db) : ITransactionRunner
{
    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct = default)
    {
        // A retry must replay the whole transaction.
        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();
            await using var transaction = await db.Database.BeginTransactionAsync(ct);
            try
            {
                var result = await operation(ct);
                await db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return result;
            }
            catch
            {
                db.ChangeTracker.Clear();
                throw;
            }
        });
    }
}
