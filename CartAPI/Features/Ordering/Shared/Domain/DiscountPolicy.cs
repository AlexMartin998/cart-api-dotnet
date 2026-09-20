namespace CartAPI.Features.Ordering.Shared.Domain;


public sealed class DiscountPolicy
{
    public DiscountPolicy(decimal minSubtotal, decimal percentage)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minSubtotal);
        ArgumentOutOfRangeException.ThrowIfNegative(percentage);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(percentage, 100m);
        MinSubtotal = minSubtotal;
        Percentage = percentage;
    }

    public decimal MinSubtotal { get; }

    public decimal Percentage { get; }

    
    public decimal For(decimal subtotal) =>
        subtotal > MinSubtotal
            ? Math.Round(subtotal * Percentage / 100m, 2, MidpointRounding.AwayFromZero)
            : 0m;
}
