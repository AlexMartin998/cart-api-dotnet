namespace CartAPI.Features.Ordering.Carts.Domain;


// aggregate root: Cart manages its CartItems and maintains aggregate consistency
public sealed class Cart
{
    public const int MaxQuantityPerLine = 100;

    private readonly List<CartItem> _items = [];

    public int Id { get; private set; }

    public int UserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    // NO cart.Items.Add(...). 
    public IReadOnlyCollection<CartItem> Items => _items;

    public bool IsEmpty => _items.Count == 0;

    private Cart() { }


    public static Cart Open(int userId, DateTime now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        return new Cart { UserId = userId, CreatedAt = now };
    }

    
    //* -------------
    public int QuantityOf(int productId) => Find(productId)?.Quantity ?? 0;

    
    public void AddOrIncrease(int productId, int quantity, DateTime now)
    {
        var line = Find(productId);
        if (line is null)
            _items.Add(new CartItem(productId, quantity));
        else
            line.Increase(quantity);

        UpdatedAt = now;
    }

    public void SetQuantity(int productId, int quantity, DateTime now)
    {
        (Find(productId) ?? throw CartErrors.LineNotFound(productId)).SetQuantity(quantity);
        UpdatedAt = now;
    }

    public void Remove(int productId, DateTime now)
    {
        _items.Remove(Find(productId) ?? throw CartErrors.LineNotFound(productId));
        UpdatedAt = now;
    }

    public void Clear(DateTime now)
    {
        if (IsEmpty)
            return;

        _items.Clear();
        UpdatedAt = now;
    }


    public IReadOnlyList<CartLine> Lines() =>
        [.. _items.OrderBy(i => i.ProductId).Select(i => new CartLine(i.ProductId, i.Quantity))];


    internal static int ValidQuantity(int quantity) =>
        quantity is < 1 or > MaxQuantityPerLine ? throw CartErrors.InvalidQuantity() : quantity;


    private CartItem? Find(int productId) => _items.Find(i => i.ProductId == productId);
}

