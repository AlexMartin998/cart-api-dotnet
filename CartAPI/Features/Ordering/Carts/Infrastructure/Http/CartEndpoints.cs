using CartAPI.Features.Ordering.Carts.Application.Queries;
using CartAPI.Shared.Infrastructure.Auth;

namespace CartAPI.Features.Ordering.Carts.Infrastructure.Http;


internal static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var cart = app.MapGroup("/api/cart").WithTags("Cart");

        cart.MapGet("/",
            async (ICurrentUser user, GetMyCartQueryHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new GetMyCartQuery(user.Id), ct))
        )
        .WithName("GetCart")
        .WithSummary("Get the current user's cart")
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
