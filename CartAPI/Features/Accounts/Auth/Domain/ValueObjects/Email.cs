namespace CartAPI.Features.Accounts.Auth.Domain.ValueObjects;

public sealed record Email
{
    public const int MaxLength = 256;

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string? email)
    {
        var normalized = email?.Trim().ToLowerInvariant();
        return normalized is { Length: > 0 and <= MaxLength } && normalized.IndexOf('@') > 0 && !normalized.EndsWith('@')
            ? new Email(normalized)
            : throw UserErrors.InvalidEmail();
    }
}
