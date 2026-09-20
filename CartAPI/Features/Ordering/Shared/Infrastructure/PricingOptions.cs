using System.ComponentModel.DataAnnotations;

namespace CartAPI.Features.Ordering.Shared.Infrastructure;


internal sealed class PricingOptions
{
    
    public const string SectionName = "Ordering:Pricing";


    [Range(typeof(decimal), "0", "1000000", ParseLimitsInInvariantCulture = true)]
    public decimal DiscountMinSubtotal { get; set; } = 100m;

    [Range(typeof(decimal), "0", "100", ParseLimitsInInvariantCulture = true)]
    public decimal DiscountPercentage { get; set; } = 10m;
}
