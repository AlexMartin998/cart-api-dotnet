using System.Text.RegularExpressions;

namespace CartAPI.Features.Catalog.Products.Domain.ValueObjects;


public sealed partial record ProductCode
{
    public const int MaxLength = 20;

    private ProductCode(string value) => Value = value;

    public string Value { get; }

    public static ProductCode Create(string? code)
    {
        var normalized = code?.Trim().ToUpperInvariant();
        return normalized is not null && Pattern().IsMatch(normalized)
            ? new ProductCode(normalized)
            : throw ProductErrors.InvalidCode(code);
    }

    [GeneratedRegex("^[A-Z0-9-]{3,20}$")]
    private static partial Regex Pattern();

}