namespace CartAPI.Shared.Application;

public interface ITransactionRunner
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct = default);
}
