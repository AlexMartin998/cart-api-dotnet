using CartAPI.Features.Accounts.Auth.Domain.ValueObjects;

namespace CartAPI.Features.Accounts.Auth.Domain;

public sealed class User
{
    public const int FullNameMaxLength = 100;

    public int Id { get; private set; }

    public string Email { get; private set; } = null!;

    public string FullName { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public string Role { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static User Register(Email email, string fullName, string role, string passwordHash, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        var name = fullName?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > FullNameMaxLength)
            throw UserErrors.InvalidFullName();
        if (!Roles.IsKnown(role))
            throw UserErrors.UnknownRole(role);

        return new User { Email = email.Value, FullName = name, Role = role, PasswordHash = passwordHash, CreatedAt = now };
    }
}
