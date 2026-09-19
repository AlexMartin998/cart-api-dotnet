using System.Text.Json;

namespace CartAPI.Shared.Infrastructure.Http.Errors;

public static class ProblemDetailsSetup
{
    public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            var problem = context.ProblemDetails;
            if (problem is HttpValidationProblemDetails validation)
            {
                validation.Errors = validation.Errors.ToDictionary(
                    e => JsonNamingPolicy.CamelCase.ConvertName(e.Key), e => e.Value);
            }

            problem.Extensions.TryAdd("code", problem is HttpValidationProblemDetails
                ? "validation_error"
                : ProblemCodes.ForStatus(problem.Status ?? context.HttpContext.Response.StatusCode));
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }
}
