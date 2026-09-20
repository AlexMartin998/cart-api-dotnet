using System.ComponentModel.DataAnnotations;
using CartAPI.Shared.Application.Paging;

namespace CartAPI.Features.Ordering.Orders.Infrastructure.Http;


public sealed record OrderHistoryParameters(
    [property: Range(1, int.MaxValue)] int? Page,
    [property: Range(1, PageRequest.MaxPageSize)] int? PageSize);

