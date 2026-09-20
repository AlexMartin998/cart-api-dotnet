namespace CartAPI.Features.Ordering.Orders.Domain;


public interface IOrderRepository
{
    void Add(Order order);
}
