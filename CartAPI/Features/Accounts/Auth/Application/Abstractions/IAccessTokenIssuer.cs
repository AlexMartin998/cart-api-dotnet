using CartAPI.Features.Accounts.Auth.Domain;

namespace CartAPI.Features.Accounts.Auth.Application.Abstractions;

public interface IAccessTokenIssuer
{
    AccessToken Issue(User user);
}

public sealed record AccessToken(string Value, DateTime ExpiresAt);
