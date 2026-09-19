using CartAPI.Features.Accounts;
using CartAPI.Features.Catalog;
using CartAPI.Persistence;
using CartAPI.Shared.Infrastructure.Auth;
using CartAPI.Shared.Infrastructure.Http;
using CartAPI.Shared.Infrastructure.Http.Errors;
using CartAPI.Shared.Infrastructure.Http.OpenApi;

namespace CartAPI.Host;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddPersistence(configuration);
        return services;
    }

    public static IServiceCollection AddFeatures(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAccountsModule(configuration);
        services.AddCatalogModule();
        return services;
    }

    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiProblemDetails();
        services.AddValidation();
        services.AddApiHttp(configuration);
        services.AddApiSecurity(configuration);
        services.AddApiDocumentation();
        return services;
    }
}
