using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Features.Ordering.Shared.Domain;


public static class OrderingErrors
{

    public static NotFoundException ProductNotFound(int productId) =>
        new("product_not_found", $"Product {productId} does not exist.");


    public static ConflictException InsufficientStock(string code, int available) =>
        new("insufficient_stock", available == 1
            ? $"Only 1 unit of {code} is left."
            : $"Only {available} units of {code} are left.");

}
