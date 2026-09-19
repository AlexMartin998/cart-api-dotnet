using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Features.Catalog.Categories.Domain;

public static class CategoryErrors
{

    public static ValidationException InvalidName() =>
        new("invalid_category_name", $"A category name needs 1 to {Category.NameMaxLength} characters.");

}
