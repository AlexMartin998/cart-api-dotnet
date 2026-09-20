namespace CartAPI.Features.Ordering.Orders.Application;


public sealed record OrderSummaryDto(int Id, DateTime PlacedAt, int ItemCount, decimal Subtotal, decimal Discount, decimal Total);
