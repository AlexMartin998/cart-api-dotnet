using System.ComponentModel.DataAnnotations;
using CartAPI.Features.Ordering.Carts.Domain;

namespace CartAPI.Features.Ordering.Carts.Infrastructure.Http;


public sealed record AddCartItemBody(
    [property: Range(1, int.MaxValue)] int ProductId,
    [property: Range(1, Cart.MaxQuantityPerLine)] int Quantity);
