using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Ordering.Carts.Application;
using CartAPI.Features.Ordering.Carts.Application.Commands;
using CartAPI.Shared.Domain.Errors;
using CartAPI.Tests.Unit.Ordering.Fakes;

namespace CartAPI.Tests.Unit.Ordering.Carts;


public sealed class AddCartItemCommandHandlerTests
{
    private readonly FakeCarts _carts = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly AddCartItemCommandHandler _handler;

    
    public AddCartItemCommandHandlerTests()
    {
        var catalog = new FakeCatalog(new ProductSnapshot(4, "SKU-004", "Monitor", 249.99m, Stock: 2));
        _handler = new AddCartItemCommandHandler(_carts, catalog, new CartPricer(catalog), _unitOfWork, TimeProvider.System);
    }


    private Task<CartDto> Add(int productId, int quantity) =>
        _handler.HandleAsync(new AddCartItemCommand(UserId: 1, productId, quantity), CancellationToken.None);

    [Fact]
    public async Task AddingTwice_SumsIntoOneLineAndPricesIt()
    {
        await Add(4, 1);
        var cart = await Add(4, 1);

        var line = Assert.Single(cart.Items);
        Assert.Equal(2, line.Quantity);
        Assert.Equal(499.98m, cart.Subtotal);
    }


    [Fact]
    public async Task TheResultingQuantity_IsWhatIsCheckedAgainstTheStock()
    {
        await Add(4, 1);

        var error = await Assert.ThrowsAsync<ConflictException>(() => Add(4, 2));

        Assert.Equal("insufficient_stock", error.Code);
        Assert.Equal(1, _carts.Stored!.QuantityOf(4));
    }


    [Fact]
    public async Task AnUnknownProduct_IsNotFound()
    {
        var error = await Assert.ThrowsAsync<NotFoundException>(() => Add(99, 1));

        Assert.Equal("product_not_found", error.Code);
        Assert.Equal(0, _unitOfWork.Saves);
    }
}
