using CartAPI.Features.Accounts.Auth.Domain.ValueObjects;

namespace CartAPI.Features.Accounts.Auth.Domain;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(Email email, CancellationToken ct);
}
