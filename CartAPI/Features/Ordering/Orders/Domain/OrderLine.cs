namespace CartAPI.Features.Ordering.Orders.Domain;


public sealed record OrderLine(int ProductId, string ProductCode, string ProductName, decimal UnitPrice, int Quantity)
{

    public decimal LineTotal => UnitPrice * Quantity;

}
