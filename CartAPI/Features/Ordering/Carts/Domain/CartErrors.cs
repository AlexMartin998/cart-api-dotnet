using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Features.Ordering.Carts.Domain;


public static class CartErrors
{
    public static ValidationException InvalidQuantity() =>
        new("invalid_quantity", $"The quantity must be between 1 and {Cart.MaxQuantityPerLine}.");


    public static NotFoundException LineNotFound(int productId) =>
        new("cart_item_not_found", $"Product {productId} is not in the cart.");
}
