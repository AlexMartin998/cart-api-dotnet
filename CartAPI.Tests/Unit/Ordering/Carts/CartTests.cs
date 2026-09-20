using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Tests.Unit.Ordering.Carts;


public sealed class CartTests
{
    private static readonly DateTime Now = new(2026, 9, 19, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void AddingTheSameProductTwice_SumsIntoOneLine()
    {
        var cart = Cart.Open(userId: 1, Now);

        cart.AddOrIncrease(productId: 7, quantity: 2, Now);
        cart.AddOrIncrease(productId: 7, quantity: 1, Now);

        var line = Assert.Single(cart.Lines());
        Assert.Equal(new CartLine(7, 3), line);
    }

    [Fact]
    public void SetQuantity_ReplacesInsteadOfAdding()
    {
        var cart = Cart.Open(1, Now);
        cart.AddOrIncrease(7, 5, Now);

        cart.SetQuantity(7, 2, Now);

        Assert.Equal(2, cart.QuantityOf(7));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(Cart.MaxQuantityPerLine + 1)]
    public void Quantities_OutOfRange_AreRejected(int quantity)
    {
        var cart = Cart.Open(1, Now);

        var error = Assert.Throws<ValidationException>(() => cart.AddOrIncrease(7, quantity, Now));
        Assert.Equal("invalid_quantity", error.Code);
    }

    [Fact]
    public void ChangingALineThatIsNotInTheCart_IsNotFound()
    {
        var cart = Cart.Open(1, Now);

        Assert.Equal("cart_item_not_found", Assert.Throws<NotFoundException>(() => cart.SetQuantity(9, 1, Now)).Code);
        Assert.Equal("cart_item_not_found", Assert.Throws<NotFoundException>(() => cart.Remove(9, Now)).Code);
    }

    [Fact]
    public void Lines_AreOrderedByProductId()
    {
        var cart = Cart.Open(1, Now);
        cart.AddOrIncrease(9, 1, Now);
        cart.AddOrIncrease(3, 1, Now);

        Assert.Equal([3, 9], cart.Lines().Select(l => l.ProductId));
    }

    [Fact]
    public void Clear_EmptiesTheCart()
    {
        var cart = Cart.Open(1, Now);
        cart.AddOrIncrease(3, 1, Now);

        cart.Clear(Now);

        Assert.True(cart.IsEmpty);
    }
}
