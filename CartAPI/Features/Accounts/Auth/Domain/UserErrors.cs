using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Features.Accounts.Auth.Domain;

public static class UserErrors
{
    public static ValidationException InvalidEmail() =>
        new("invalid_email", "The email is not valid.");

    public static ValidationException InvalidFullName() =>
        new("invalid_full_name", $"The full name needs 1 to {User.FullNameMaxLength} characters.");

    public static ValidationException UnknownRole(string role) =>
        new("unknown_role", $"'{role}' is not a known role.");

    public static NotFoundException NotFound() =>
        new("user_not_found", "The user does not exist.");

    // Same error for unknown email and wrong password: no account enumeration.
    public static UnauthorizedException InvalidCredentials() =>
        new("invalid_credentials", "Invalid email or password.");
}
