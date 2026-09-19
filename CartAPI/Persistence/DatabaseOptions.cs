using System.ComponentModel.DataAnnotations;

namespace CartAPI.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required(ErrorMessage = "ConnectionStrings:Default is required.")]
    public string ConnectionString { get; set; } = string.Empty;

    public bool MigrateOnStartup { get; set; } = true;
}
