using CartAPI.Features.Catalog.Contracts;
using CartAPI.Features.Ordering.Carts.Domain;

namespace CartAPI.Features.Ordering.Carts.Application;


// service application ---
public sealed class CartPricer(ICatalogStock catalog)
{
    public async Task<CartDto> PriceAsync(Cart? cart, CancellationToken ct)
    {
        if (cart is null || cart.IsEmpty)
            return CartDto.Empty;

        var lines = cart.Lines();
        var snapshots = await catalog.PeekAsync([.. lines.Select(l => l.ProductId)], ct);
        var items = lines.Select(line => Price(line, snapshots.GetValueOrDefault(line.ProductId))).ToList();

        var subtotal = items.Where(i => i.Status == CartItemStatus.Ok).Sum(i => i.LineTotal);
        const decimal discount = 0m;
        return new CartDto(items, items.Sum(i => i.Quantity), subtotal, discount, subtotal - discount);
    }

    private static CartItemDto Price(CartLine line, ProductSnapshot? product)
    {
        if (product is null)
            return new CartItemDto(line.ProductId, null, null, 0m, line.Quantity, 0m, 0, CartItemStatus.Unavailable);

        var status = product.Stock >= line.Quantity ? CartItemStatus.Ok : CartItemStatus.InsufficientStock;
        return new CartItemDto(
            product.ProductId, product.Code, product.Name, product.UnitPrice, line.Quantity,
            product.UnitPrice * line.Quantity, product.Stock, status);
    }
}
