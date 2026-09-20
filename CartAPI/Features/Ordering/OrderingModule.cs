using CartAPI.Features.Ordering.Carts.Application;
using CartAPI.Features.Ordering.Carts.Application.Commands;
using CartAPI.Features.Ordering.Carts.Application.Queries;
using CartAPI.Features.Ordering.Carts.Domain;
using CartAPI.Features.Ordering.Carts.Infrastructure.Http;
using CartAPI.Features.Ordering.Carts.Infrastructure.Persistence;
using CartAPI.Features.Ordering.Carts.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Ordering;


public static class OrderingModule
{
    public static IServiceCollection AddOrderingModule(this IServiceCollection services)
    {
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<CartPricer>();
        services.AddScoped<GetMyCartQueryHandler>();
        services.AddScoped<AddCartItemCommandHandler>();
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
