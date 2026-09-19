using CartAPI.Features.Accounts.Auth.Domain;
using CartAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Accounts.Auth.Application.Queries;

public sealed record GetMeQuery(int UserId);

public sealed class GetMeQueryHandler(AppDbContext db)
{
    public async Task<UserDto> HandleAsync(GetMeQuery query, CancellationToken ct) =>
        await db.Set<User>().AsNoTracking()
            .Where(u => u.Id == query.UserId)
            .Select(u => new UserDto(u.Id, u.Email, u.FullName, u.Role))
            .FirstOrDefaultAsync(ct)
        ?? throw UserErrors.NotFound();
}
