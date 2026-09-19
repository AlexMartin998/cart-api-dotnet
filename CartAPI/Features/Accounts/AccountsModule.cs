using CartAPI.Features.Accounts.Auth.Application.Abstractions;
using CartAPI.Features.Accounts.Auth.Application.Commands;
using CartAPI.Features.Accounts.Auth.Application.Queries;
using CartAPI.Features.Accounts.Auth.Domain;
using CartAPI.Features.Accounts.Auth.Infrastructure.Http;
using CartAPI.Features.Accounts.Auth.Infrastructure.Persistence;
using CartAPI.Features.Accounts.Auth.Infrastructure.Persistence.Configurations;
using CartAPI.Features.Accounts.Auth.Infrastructure.Security;
using CartAPI.Features.Accounts.Auth.Infrastructure.Seeding;
using CartAPI.Shared.Infrastructure.Auth;
using CartAPI.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Accounts;

public static class AccountsModule
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<GetMeQueryHandler>();
        services.AddAuthorizationBuilder().AddPolicy(Policies.Admin, p => p.RequireRole(Roles.Admin));

        services.AddOptions<UserSeedOptions>()
            .Bind(configuration.GetSection(UserSeedOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IDataSeeder, UserSeeder>();
        return services;
    }

    public static void ConfigureModel(ModelBuilder builder) =>
        builder.ApplyConfiguration(new UserConfiguration());

    public static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapAuthEndpoints();
}
