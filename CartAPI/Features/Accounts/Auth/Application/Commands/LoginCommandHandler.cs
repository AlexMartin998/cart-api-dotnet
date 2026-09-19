using CartAPI.Features.Accounts.Auth.Application.Abstractions;
using CartAPI.Features.Accounts.Auth.Domain;
using CartAPI.Features.Accounts.Auth.Domain.ValueObjects;

namespace CartAPI.Features.Accounts.Auth.Application.Commands;

public sealed record LoginCommand(string Email, string Password);

public sealed record LoginResult(string AccessToken, string TokenType, DateTime ExpiresAt, UserDto User);

public sealed class LoginCommandHandler(IUserRepository users, IPasswordHasher hasher, IAccessTokenIssuer tokens)
{
    public async Task<LoginResult> HandleAsync(LoginCommand command, CancellationToken ct)
    {
        var user = await users.FindByEmailAsync(Email.Create(command.Email), ct);
        if (user is null || !hasher.Verify(user.PasswordHash, command.Password))
            throw UserErrors.InvalidCredentials();

        var token = tokens.Issue(user);
        return new LoginResult(token.Value, "Bearer", token.ExpiresAt, UserDto.From(user));
    }
}
