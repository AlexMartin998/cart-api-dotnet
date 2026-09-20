using CartAPI.Features.Ordering.Carts.Application;
using CartAPI.Features.Ordering.Carts.Application.Commands;
using CartAPI.Features.Ordering.Carts.Application.Queries;
using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Features.Ordering.Carts.Infrastructure.Http;
using CartAPI.Features.Ordering.Carts.Infrastructure.Persistence;
using CartAPI.Features.Ordering.Carts.Infrastructure.Persistence.Configurations;
using CartAPI.Features.Ordering.Shared.Domain;
using CartAPI.Features.Ordering.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CartAPI.Features.Ordering;


public static class OrderingModule
{
    public static IServiceCollection AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
    {
        // -------
        // startup fails
        services.AddOptions<PricingOptions>()
            .Bind(configuration.GetSection(PricingOptions.SectionName)) // appsettings.json: Ordering:Pricing
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PricingOptions>>().Value;
            return new OrderPricing(new DiscountPolicy(options.DiscountMinSubtotal, options.DiscountPercentage));
        });

        // -------
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<CartPricer>();
        services.AddScoped<GetMyCartQueryHandler>();
        services.AddScoped<AddCartItemCommandHandler>();
        services.AddScoped<SetCartItemQuantityCommandHandler>();
        services.AddScoped<RemoveCartItemCommandHandler>();
        services.AddScoped<ClearCartCommandHandler>();
        return services;
    }

    public static void ConfigureModel(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CartConfiguration());
        builder.ApplyConfiguration(new CartItemConfiguration());
    }

    public static IEndpointRouteBuilder MapOrderingEndpoints(this IEndpointRouteBuilder app) =>
        app.MapCartEndpoints();
}
