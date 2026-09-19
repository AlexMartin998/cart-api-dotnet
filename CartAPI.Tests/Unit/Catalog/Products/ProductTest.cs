using CartAPI.Features.Catalog.Products.Domain;
using CartAPI.Features.Catalog.Products.Domain.ValueObjects;
using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Tests.Unit.Catalog.Products;


public sealed class ProductTests
{
    private static readonly DateTime Now = new(2026, 9, 19, 12, 0, 0, DateTimeKind.Utc);

    
    private static Product NewProduct(decimal price = 25m, int stock = 10) =>
        Product.Create(ProductCode.Create("SKU-001"), " Teclado ", null, price, stock, categoryId: 1, Now);


    [Fact]
    public void Create_StartsActiveAndTrimsTheName()
    {
        var product = NewProduct();

        Assert.True(product.IsActive);
        Assert.Equal("Teclado", product.Name);
        Assert.Equal("SKU-001", product.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10.001)]
    public void Create_RejectsInvalidPrices(decimal price)
    {
        var error = Assert.Throws<ValidationException>(() => NewProduct(price: price));
        Assert.Equal("invalid_price", error.Code);
    }

    [Fact]
    public void Create_RejectsNegativeStock()
    {
        var error = Assert.Throws<ValidationException>(() => NewProduct(stock: -1));
        Assert.Equal("invalid_stock", error.Code);
    }

    [Fact]
    public void Deactivate_KeepsTheProductButTakesItOutOfSale()
    {
        var product = NewProduct();

        product.Deactivate(Now);

        Assert.False(product.IsActive);
        Assert.Equal(Now, product.UpdatedAt);
    }
}

// dotnet test --filter "FullyQualifiedName~ProductTests"
// dotnet test --filter "FullyQualifiedName~ProductTests" --logger "console;verbosity=normal"
