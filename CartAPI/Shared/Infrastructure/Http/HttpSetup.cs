using Microsoft.AspNetCore.HttpLogging;

namespace CartAPI.Shared.Infrastructure.Http;

public static class HttpSetup
{
    public const string CorsPolicy = "Frontend";

    public static IServiceCollection AddApiHttp(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration arrays merge by index: keep the base list empty.
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options => options.AddPolicy(CorsPolicy, policy => policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Location")));

        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath
                | HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.Duration;
            options.CombineLogs = true;
        });
        return services;
    }
}
