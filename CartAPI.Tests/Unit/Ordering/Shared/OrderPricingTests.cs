using CartAPI.Features.Ordering.Shared.Domain;

namespace CartAPI.Tests.Unit.Ordering.Shared;


public sealed class OrderPricingTests
{
    
    private readonly OrderPricing _pricing = new(new DiscountPolicy(minSubtotal: 100m, percentage: 10m));

    [Theory]
    [InlineData(60.00)]
    [InlineData(100.00)]
    public void AtOrBelowTheThreshold_ThereIsNoDiscount(decimal subtotal)
    {
        var totals = _pricing.For([subtotal]);

        Assert.Equal(0m, totals.Discount);
        Assert.Equal(subtotal, totals.Total);
    }

    [Fact]
    public void AboveTheThreshold_TenPercentIsDiscounted()
    {
        var totals = _pricing.For([100.00m, 15.00m]);

        Assert.Equal(new OrderTotals(115.00m, 11.50m, 103.50m), totals);
    }

    [Fact]
    public void JustAboveTheThreshold_AlreadyDiscounts() =>
        Assert.Equal(10.00m, _pricing.For([100.01m]).Discount);

    // 100,05 * 10 % = 10,005: AwayFromZero da 10,01
    [Fact]
    public void TheDiscountIsRoundedAwayFromZero() =>
        Assert.Equal(10.01m, _pricing.For([100.05m]).Discount);
}
