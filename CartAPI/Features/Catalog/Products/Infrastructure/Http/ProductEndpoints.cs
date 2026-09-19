using CartAPI.Features.Catalog.Products.Application.Queries;
using CartAPI.Shared.Application.Paging;

namespace CartAPI.Features.Catalog.Products.Infrastructure.Http;



internal static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {

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

        return app;
    }
}
