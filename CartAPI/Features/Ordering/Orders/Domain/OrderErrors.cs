using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Features.Ordering.Orders.Domain;


public static class OrderErrors
{

    public static ConflictException CartEmpty() =>
        new("cart_empty", "The cart is empty: there is nothing to buy.");

    public static ValidationException NoLines() =>
        new("order_without_lines", "An order needs at least one line.");

    public static NotFoundException NotFound(int orderId) =>
        new("order_not_found", $"Order {orderId} does not exist.");

}
