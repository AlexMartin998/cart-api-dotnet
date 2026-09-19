using CartAPI.Features.Accounts.Auth.Application.Abstractions;
using CartAPI.Features.Accounts.Auth.Domain;
using CartAPI.Features.Accounts.Auth.Domain.ValueObjects;
using CartAPI.Persistence;
using CartAPI.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CartAPI.Features.Accounts.Auth.Infrastructure.Seeding;

internal sealed class UserSeeder(AppDbContext db, IPasswordHasher hasher, IOptions<UserSeedOptions> options, TimeProvider clock)
    : IDataSeeder
{
    public int Order => 0;

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await db.Set<User>().AnyAsync(ct))
            return;

        var seed = options.Value;
        var now = clock.GetUtcNow().UtcDateTime;
        db.Set<User>().AddRange(
            User.Register(Email.Create(seed.AdminEmail), "Administrador", Roles.Admin, hasher.Hash(seed.AdminPassword), now),
            User.Register(Email.Create(seed.CustomerEmail), "Cliente de prueba", Roles.Customer, hasher.Hash(seed.CustomerPassword), now));
        await db.SaveChangesAsync(ct);
    }
}
