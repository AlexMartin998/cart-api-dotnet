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
