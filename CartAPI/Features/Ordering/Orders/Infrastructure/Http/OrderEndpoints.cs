using CartAPI.Features.Ordering.Orders.Application.Commands;
using CartAPI.Shared.Infrastructure.Auth;

namespace CartAPI.Features.Ordering.Orders.Infrastructure.Http;


internal static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/api/orders").WithTags("Orders");


        orders.MapPost("/", async (ICurrentUser user, PlaceOrderCommandHandler handler, CancellationToken ct) =>
            {
                var order = await handler.HandleAsync(new PlaceOrderCommand(user.Id), ct);
                return TypedResults.Created($"/api/orders/{order.Id}", order);
            })
            .WithName("PlaceOrder")
            .WithSummary("Places an order for the current user based on their cart.")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }
}
