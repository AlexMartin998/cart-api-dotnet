using CartAPI.Features.Catalog.Categories.Domain;
using CartAPI.Persistence;
using CartAPI.Shared.Application.Paging;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Features.Catalog.Products.Application.Queries;


public sealed record SearchProductsQuery(
    string? Search, int? CategoryId, decimal? MinPrice, decimal? MaxPrice, bool? InStock, PageRequest Page);


public sealed class SearchProductsQueryHandler(AppDbContext db)
{

    public Task<PagedResult<ProductDto>> HandleAsync(SearchProductsQuery query, CancellationToken ct)
    {
        var products = db.ActiveProducts().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            products = products.Where(p => p.Name.Contains(term)
                || p.Code.Contains(term)
                || db.Set<Category>().Any(c => c.Id == p.CategoryId && c.Name.Contains(term)));
        }


        if (query.CategoryId is { } categoryId)
            products = products.Where(p => p.CategoryId == categoryId);
        if (query.MinPrice is { } min)
            products = products.Where(p => p.Price >= min);
        if (query.MaxPrice is { } max)
            products = products.Where(p => p.Price <= max);
        if (query.InStock is true)
            products = products.Where(p => p.Stock > 0);


        return products
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .ToDtos(db)
            .ToPagedResultAsync(query.Page, ct);
    }
}
