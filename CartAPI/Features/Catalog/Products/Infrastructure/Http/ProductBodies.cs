using System.ComponentModel.DataAnnotations;
using CartAPI.Shared.Application.Paging;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Http;


public sealed record SearchProductsParameters(
    [property: MaxLength(100)] string? Search,
    [property: Range(1, int.MaxValue)] int? CategoryId,
    [property: Range(typeof(decimal), "0", "1000000", ParseLimitsInInvariantCulture = true)] decimal? MinPrice,
    [property: Range(typeof(decimal), "0", "1000000", ParseLimitsInInvariantCulture = true)] decimal? MaxPrice,
    bool? InStock,
    [property: Range(1, int.MaxValue)] int? Page,
    [property: Range(1, PageRequest.MaxPageSize)] int? PageSize) : IValidatableObject
{
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPrice > MaxPrice)
            yield return new ValidationResult("minPrice can't be greater than maxPrice.", [nameof(MinPrice)]);
    }

}


public sealed record CreateProductBody(
    [property: Required, MaxLength(20)] string Code,
    [property: Required, MaxLength(120)] string Name,
    [property: MaxLength(500)] string? Description,
    [property: Range(typeof(decimal), "0.01", "1000000", ParseLimitsInInvariantCulture = true)] decimal Price,
    [property: Range(0, 1_000_000)] int Stock,
    [property: Range(1, int.MaxValue)] int CategoryId);

public sealed record UpdateProductBody(
    [property: Required, MaxLength(120)] string Name,
    [property: MaxLength(500)] string? Description,
    [property: Range(typeof(decimal), "0.01", "1000000", ParseLimitsInInvariantCulture = true)] decimal Price,
    [property: Required, Range(0, 1_000_000)] int? Stock,
    [property: Range(1, int.MaxValue)] int CategoryId);

