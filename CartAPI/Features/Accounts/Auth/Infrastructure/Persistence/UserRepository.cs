using CartAPI.Features.Accounts.Auth.Domain;
using CartAPI.Features.Accounts.Auth.Domain.ValueObjects;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Accounts.Auth.Infrastructure.Persistence;

internal sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> FindByEmailAsync(Email email, CancellationToken ct) =>
        db.Set<User>().FirstOrDefaultAsync(u => u.Email == email.Value, ct);
}
