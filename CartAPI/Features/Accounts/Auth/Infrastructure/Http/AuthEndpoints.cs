using CartAPI.Features.Accounts.Auth.Application.Commands;
using CartAPI.Features.Accounts.Auth.Application.Queries;
using CartAPI.Shared.Infrastructure.Auth;

namespace CartAPI.Features.Accounts.Auth.Infrastructure.Http;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth").WithTags("Auth");

        auth.MapPost("/login", async (LoginBody body, LoginCommandHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new LoginCommand(body.Email, body.Password), ct)))
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Autentica al usuario y devuelve un JWT.")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        auth.MapGet("/me", async (ICurrentUser user, GetMeQueryHandler handler, CancellationToken ct) =>
                TypedResults.Ok(await handler.HandleAsync(new GetMeQuery(user.Id), ct)))
            .WithName("GetMe")
            .WithSummary("Datos del usuario autenticado.")
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
