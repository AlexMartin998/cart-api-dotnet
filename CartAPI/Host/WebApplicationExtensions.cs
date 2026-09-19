using CartAPI.Features.Accounts;
using CartAPI.Features.Catalog;
using CartAPI.Shared.Infrastructure.Http;
using CartAPI.Shared.Infrastructure.Http.OpenApi;

namespace CartAPI.Host;


public static class WebApplicationExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseHttpLogging();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseCors(HttpSetup.CorsPolicy);
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    public static WebApplication MapFeatures(this WebApplication app)
    {
        app.MapApiDocumentation();
        app.MapAccountsEndpoints();
        app.MapCatalogEndpoints();
        return app;
    }
}
