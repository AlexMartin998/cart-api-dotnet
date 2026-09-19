using CartAPI.Features.Catalog.Products.Domain.ValueObjects;
using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Features.Catalog.Products.Domain;


public static class ProductErrors
{
    public static ValidationException InvalidCode(string? code) =>
        new("invalid_product_code", $"'{code}' is not a valid code: 3 to {ProductCode.MaxLength} letters, digits or dashes.");

    public static ValidationException InvalidName() =>
        new("invalid_product_name", $"A product name needs 1 to {Product.NameMaxLength} characters.");

    public static ValidationException InvalidDescription() =>
        new("invalid_product_description", $"A description can't be longer than {Product.DescriptionMaxLength} characters.");

    public static ValidationException InvalidPrice() =>
        new("invalid_price", "The price must be greater than 0 with at most 2 decimals.");

    public static ValidationException InvalidStock() =>
        new("invalid_stock", "The stock can't be negative.");

    public static ValidationException InvalidCategory() =>
        new("invalid_category", "The product needs a category.");
}
