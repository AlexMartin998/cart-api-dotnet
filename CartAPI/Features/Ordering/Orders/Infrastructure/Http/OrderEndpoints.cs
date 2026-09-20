using CartAPI.Features.Ordering.Orders.Application.Commands;
using CartAPI.Shared.Infrastructure.Auth;
using CartAPI.Features.Ordering.Orders.Application.Queries;
using CartAPI.Shared.Application.Paging;

namespace CartAPI.Features.Ordering.Orders.Infrastructure.Http;


internal static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/api/orders").WithTags("Orders");


        // ------------
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


        // ------------
        orders.MapGet("/",
            async ([AsParameters] OrderHistoryParameters p, ICurrentUser user, ListMyOrdersQueryHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new ListMyOrdersQuery(user.Id, new PageRequest(p.Page, p.PageSize)), ct))
        )
        .WithName("ListMyOrders")
        .WithSummary("List the current user's orders with pagination.")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);


        // ------------
        orders.MapGet("/{id:int}",
            async (int id, ICurrentUser user, GetMyOrderQueryHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new GetMyOrderQuery(user.Id, id), ct))
        )
        .WithName("GetMyOrder")
        .WithSummary("Get the details of a specific order for the current user.")
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);


        return app;
    }
}
