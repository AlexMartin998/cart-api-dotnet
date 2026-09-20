using CartAPI.Features.Catalog.Categories.Application.Queries;

namespace CartAPI.Features.Catalog.Categories.Infrastructure.Http;


internal static class CategoryEndpoints
{
    
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/categories").WithTags("Categories")
            .MapGet("/", async (ListCategoriesQueryHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(ct)))
            .WithName("ListCategories")
            .WithSummary("Categorías para el filtro del catálogo.")
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }

}   
