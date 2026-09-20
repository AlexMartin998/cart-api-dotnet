using CartAPI.Features.Catalog.Products.Application.Queries;
using CartAPI.Features.Catalog.Products.Application.Commands;
using CartAPI.Shared.Infrastructure.Auth;
using CartAPI.Shared.Application.Paging;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Http;



internal static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {

        // ------------
        var products = app.MapGroup("/api/products").WithTags("Products");

        products.MapGet("/",
            async ([AsParameters] SearchProductsParameters p, SearchProductsQueryHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(
                    new SearchProductsQuery(p.Search, p.CategoryId, p.MinPrice, p.MaxPrice, p.InStock, new PageRequest(p.Page, p.PageSize)),
                    ct)
                )
        )
        .WithName("SearchProducts")
        .WithSummary("Products list by search, category, price range and stock availability.")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        
        products.MapGet("/{id:int}",
            async(int id, GetProductQueryHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new GetProductQuery(id), ct))
        )
        .WithName("GetProduct")
        .WithSummary("Get a product by id.")
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);



        // ------------
        var admin = products.MapGroup("/").RequireAuthorization(Policies.Admin);

        admin.MapPost("/", async (CreateProductBody body, CreateProductCommandHandler handler, CancellationToken ct) =>
            {
                var product = await handler.HandleAsync(
                    new CreateProductCommand(body.Code, body.Name, body.Description, body.Price, body.Stock, body.CategoryId), ct);
                return TypedResults.Created($"/api/products/{product.Id}", product);
            }
        )
        .WithName("CreateProduct")
        .WithSummary("Admin: Create a new product.")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status409Conflict);


        admin.MapPut("/{id:int}", async (int id, UpdateProductBody body, UpdateProductCommandHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(
                    new UpdateProductCommand(id, body.Name, body.Description, body.Price, body.Stock!.Value, body.CategoryId), ct))
        )
        .WithName("UpdateProduct")
        .WithSummary("Admin: update a product.")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);


        admin.MapDelete("/{id:int}", async (int id, DeactivateProductCommandHandler handler, CancellationToken ct) =>
            {
                await handler.HandleAsync(new DeactivateProductCommand(id), ct);
                return TypedResults.NoContent();
            }
        )
        .WithName("DeactivateProduct")
        .WithSummary("Admin: deactivate a product.")
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);


        return app;
    }

}
