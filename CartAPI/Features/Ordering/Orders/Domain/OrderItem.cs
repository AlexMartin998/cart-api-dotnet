namespace CartAPI.Features.Ordering.Orders.Domain;


public sealed class OrderItem
{
    public const int ProductCodeMaxLength = 20;
    public const int ProductNameMaxLength = 120;

    public int Id { get; private set; }

    public int OrderId { get; private set; }

    public int ProductId { get; private set; }

    public string ProductCode { get; private set; } = null!;

    public string ProductName { get; private set; } = null!;

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal LineTotal { get; private set; }

    private OrderItem() { }

    internal OrderItem(OrderLine line)
    {
        ProductId = line.ProductId;
        ProductCode = line.ProductCode;
        ProductName = line.ProductName;
        UnitPrice = line.UnitPrice;
        Quantity = line.Quantity;
        LineTotal = line.LineTotal;
    }
}
