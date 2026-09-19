using System.ComponentModel.DataAnnotations;

namespace CartAPI.Shared.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jwt:Key is required.")]
    [MinLength(32, ErrorMessage = "Jwt:Key must have at least 32 characters.")]
    public string Key { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int ExpiresMinutes { get; set; } = 60;
}
