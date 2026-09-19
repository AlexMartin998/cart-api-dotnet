namespace CartAPI.Features.Catalog.Categories.Domain;


public sealed class Category
{

    public const int NameMaxLength = 60;

    public int Id { get; private set; }

    public string Name { get; private set; } = null!;


    private Category() { }


    public static Category Create(string name)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrEmpty(trimmedName) || trimmedName.Length > NameMaxLength)
            throw CategoryErrors.InvalidName();

        return new Category { Name = trimmedName };

    }

}
