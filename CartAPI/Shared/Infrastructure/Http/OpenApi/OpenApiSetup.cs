namespace CartAPI.Shared.Infrastructure.Http.OpenApi;

public static class OpenApiSetup
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecurityTransformer>();
            options.AddOperationTransformer<BearerSecurityTransformer>();
            options.AddSchemaTransformer<NumbersAsNumbersTransformer>();
        });
        return services;
    }

    public static WebApplication MapApiDocumentation(this WebApplication app)
    {
        app.MapOpenApi().AllowAnonymous();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "CartAPI v1");
            options.DocumentTitle = "CartAPI";
        });
        return app;
    }
}
