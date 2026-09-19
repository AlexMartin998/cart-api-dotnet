using System.ComponentModel.DataAnnotations;

namespace CartAPI.Features.Accounts.Auth.Infrastructure.Seeding;

internal sealed class UserSeedOptions
{
    public const string SectionName = "Seed";

    [Required, EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string AdminPassword { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string CustomerPassword { get; set; } = string.Empty;
}
