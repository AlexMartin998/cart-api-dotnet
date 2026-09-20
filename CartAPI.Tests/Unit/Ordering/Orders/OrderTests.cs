using CartAPI.Features.Ordering.Orders.Domain;
using CartAPI.Features.Ordering.Shared.Domain;
using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Tests.Unit.Ordering.Orders;


public sealed class OrderTests
{
    private static readonly OrderPricing Pricing = new(new DiscountPolicy(100m, 10m));
    private static readonly DateTime Now = new(2026, 9, 19, 12, 0, 0, DateTimeKind.Utc);


    [Fact]
    public void Place_FreezesPricesAndAppliesTheSameRuleAsTheCart()
    {
        var order = Order.Place(userId: 2,
            [new OrderLine(8, "SKU-008", "Camiseta", 15.00m, 5), new OrderLine(12, "SKU-012", "Esterilla", 21.40m, 2)],
            Pricing, Now);

        Assert.Equal(117.80m, order.Subtotal);
        Assert.Equal(11.78m, order.Discount);
        Assert.Equal(106.02m, order.Total);
        Assert.Contains(order.Items, i => i is { ProductCode: "SKU-008", UnitPrice: 15.00m, Quantity: 5, LineTotal: 75.00m });
    }


    [Fact]
    public void Place_WithoutLines_IsRejected() =>
        Assert.Equal("order_without_lines", Assert.Throws<ValidationException>(() => Order.Place(2, [], Pricing, Now)).Code);

}
