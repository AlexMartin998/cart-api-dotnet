using CartAPI.Features.Catalog.Products.Domain.ValueObjects;

namespace CartAPI.Features.Catalog.Products.Domain;


public sealed class Product
{
    public const int NameMaxLength = 120;
    public const int DescriptionMaxLength = 500;

    public int Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public int CategoryId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private Product() { }


    //* -------------
    public static Product Create(
        ProductCode code, string name, string? description, decimal price, int stock, int categoryId, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(code);

        var product = new Product { Code = code.Value, IsActive = true, CreatedAt = now };
        product.Apply(name, description, price, stock, categoryId);
        return product;
    }


    public void Update(string name, string? description, decimal price, int stock, int categoryId, DateTime now)
    {
        Apply(name, description, price, stock, categoryId);
        UpdatedAt = now;
    }

    public void Deactivate(DateTime now)
    {
        if (!IsActive)
            return;

        IsActive = false;
        UpdatedAt = now;
    }

    private void Apply(string name, string? description, decimal price, int stock, int categoryId)
    {
        var trimmedName = name?.Trim();
        if (string.IsNullOrEmpty(trimmedName) || trimmedName.Length > NameMaxLength)
            throw ProductErrors.InvalidName();

        var trimmedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (trimmedDescription?.Length > DescriptionMaxLength)
            throw ProductErrors.InvalidDescription();

        if (price <= 0 || decimal.Round(price, 2) != price)
            throw ProductErrors.InvalidPrice();
        if (stock < 0)
            throw ProductErrors.InvalidStock();
        if (categoryId <= 0)
            throw ProductErrors.InvalidCategory();


        Name = trimmedName;
        Description = trimmedDescription;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
    }

}
