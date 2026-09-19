using CartAPI.Features.Catalog.Products.Domain.ValueObjects;
using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Tests.Unit.Catalog.Products;


public sealed class ProductCodeTests
{
    [Fact]
    public void Create_NormalizesToUpperCaseWithoutSpaces() =>
        Assert.Equal("SKU-001", ProductCode.Create("  sku-001 ").Value);

    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("AB")]
    [InlineData("SKU 001")]
    [InlineData("SKU-00000000000000001")]
    public void Create_RejectsInvalidCodes(string? code)
    {
        var error = Assert.Throws<ValidationException>(() => ProductCode.Create(code));
        Assert.Equal("invalid_product_code", error.Code);
    }

    [Fact]
    public void TwoCodesWithTheSameValueAreEqual() =>
        Assert.Equal(ProductCode.Create("sku-001"), ProductCode.Create("SKU-001"));

}

// dotnet test --filter "FullyQualifiedName~ProductCodeTests"
// dotnet test --filter "FullyQualifiedName~ProductCodeTests" --logger "console;verbosity=normal"
