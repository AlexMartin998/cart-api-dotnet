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


        // ------------
        cart.MapPut("/items/{productId:int}",
            async (
                int productId, SetCartItemQuantityBody body, ICurrentUser user,
                SetCartItemQuantityCommandHandler handler, CancellationToken ct
            ) => TypedResults.Ok(await handler.HandleAsync(new SetCartItemQuantityCommand(user.Id, productId, body.Quantity), ct))
        )
        .WithName("SetCartItemQuantity")
        .WithSummary("Set the quantity of an item in the current user's cart")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);


        // ------------
        cart.MapDelete("/items/{productId:int}",
            async (
                int productId, ICurrentUser user, RemoveCartItemCommandHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new RemoveCartItemCommand(user.Id, productId), ct))
        )
        .WithName("RemoveCartItem")
        .WithSummary("Remove an item from the current user's cart")
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);


        // ------------
        cart.MapDelete("/",
            async (ICurrentUser user, ClearCartCommandHandler handler, CancellationToken ct) =>
            {
                await handler.HandleAsync(new ClearCartCommand(user.Id), ct);
                return TypedResults.NoContent();
            }
        )
        .WithName("ClearCart")
        .WithSummary("Clear the current user's cart")
        .ProducesProblem(StatusCodes.Status401Unauthorized);


        return app;
    }
}
