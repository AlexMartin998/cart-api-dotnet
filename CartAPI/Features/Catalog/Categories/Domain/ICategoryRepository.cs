namespace CartAPI.Features.Catalog.Categories.Domain;


public interface ICategoryRepository
{

    Task<Category?> FindAsync(int categoryId, CancellationToken ct);

}
