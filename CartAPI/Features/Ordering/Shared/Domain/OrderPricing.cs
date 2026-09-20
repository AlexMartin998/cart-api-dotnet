namespace CartAPI.Features.Ordering.Shared.Domain;


// point of truth: one single place where we sum and discount for both carts and orders
public sealed class OrderPricing(DiscountPolicy discount)
{
    public OrderTotals For(IEnumerable<decimal> lineTotals)
    {
        var subtotal = lineTotals.Sum();
        var discountAmount = discount.For(subtotal);
        return new OrderTotals(subtotal, discountAmount, subtotal - discountAmount);
    }
}



public sealed record OrderTotals(decimal Subtotal, decimal Discount, decimal Total);
