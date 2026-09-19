using CartAPI.Features.Accounts.Auth.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace CartAPI.Features.Accounts.Auth.Infrastructure.Security;

internal sealed class IdentityPasswordHasher : IPasswordHasher
{
    private static readonly PasswordHasher<object> Hasher = new();

    // The default hasher ignores the user instance.
    private static readonly object AnyUser = new();

    public string Hash(string password) => Hasher.HashPassword(AnyUser, password);

    public bool Verify(string passwordHash, string password) =>
        Hasher.VerifyHashedPassword(AnyUser, passwordHash, password) is not PasswordVerificationResult.Failed;
}
