namespace CartAPI.Features.Ordering.Carts.Domain;


public sealed class CartItem
{
    public int Id { get; private set; }

    public int CartId { get; private set; }

    public int ProductId { get; private set; }

    public int Quantity { get; private set; }

    private CartItem() { }

    // only Cart can create a CartItem, to ensure aggregate consistency
    internal CartItem(int productId, int quantity)
    {
        ProductId = productId;
        Quantity = Cart.ValidQuantity(quantity);
    }

    
    internal void Increase(int quantity) => Quantity = Cart.ValidQuantity(Quantity + quantity);

    internal void SetQuantity(int quantity) => Quantity = Cart.ValidQuantity(quantity);
}

