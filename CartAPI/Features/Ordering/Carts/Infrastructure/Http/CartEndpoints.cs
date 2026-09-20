using CartAPI.Features.Ordering.Carts.Application.Commands;
using CartAPI.Features.Ordering.Carts.Application.Queries;
using CartAPI.Shared.Infrastructure.Auth;

namespace CartAPI.Features.Ordering.Carts.Infrastructure.Http;


internal static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var cart = app.MapGroup("/api/cart").WithTags("Cart");


        // ------------
        cart.MapGet("/",
            async (ICurrentUser user, GetMyCartQueryHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new GetMyCartQuery(user.Id), ct))
        )
        .WithName("GetCart")
        .WithSummary("Get the current user's cart")
        .ProducesProblem(StatusCodes.Status401Unauthorized);


        // ------------
        cart.MapPost("/items",
            async (AddCartItemBody body, ICurrentUser user, AddCartItemCommandHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new AddCartItemCommand(user.Id, body.ProductId, body.Quantity), ct))
        )
        .WithName("AddCartItem")
        .WithSummary("Add an item to the current user's cart")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }
}
