using System.ComponentModel.DataAnnotations;

namespace CartAPI.Features.Accounts.Auth.Infrastructure.Http;

public sealed record LoginBody(
    [property: Required, EmailAddress, MaxLength(256)] string Email,
    [property: Required, MaxLength(128)] string Password);
