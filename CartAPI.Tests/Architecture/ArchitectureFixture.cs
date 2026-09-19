using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using CartAPI.Host;

namespace CartAPI.Tests.Architecture;

internal static class ArchitectureFixture
{
    public static readonly ArchUnitNET.Domain.Architecture Api =
        new ArchLoader().LoadAssemblies(typeof(ServiceCollectionExtensions).Assembly).Build();

    public const string AnyDomain = @"^CartAPI\.(Features\.\w+\.\w+|Shared)\.Domain(\.|$)";
    public const string AnyApplication = @"^CartAPI\.(Features\.\w+\.\w+|Shared)\.Application(\.|$)";
    public const string AnyInfrastructure = @"^CartAPI\.(Features\.\w+\.\w+|Shared)\.Infrastructure(\.|$)";
    public const string AnyFeatureInfrastructure = @"^CartAPI\.Features\.\w+\.\w+\.Infrastructure(\.|$)";
    public const string AnyHttpAdapter = @"^CartAPI\.Features\.\w+\.\w+\.Infrastructure\.Http(\.|$)";
    public const string AnyContracts = @"^CartAPI\.Features\.\w+\.Contracts(\.|$)";
    public const string FeaturesNamespace = @"^CartAPI\.Features\.";
    public const string SharedNamespace = @"^CartAPI\.Shared(\.|$)";
    public const string PersistenceNamespace = @"^CartAPI\.Persistence(\.|$)";
    public const string HostNamespace = @"^CartAPI\.Host(\.|$)";
    public const string EntityFramework = @"^Microsoft\.EntityFrameworkCore(\.|$)";
    public const string AspNetCore = @"^Microsoft\.AspNetCore(\.|$)";

    public const string ContextShared = "Shared";

    public static readonly string[] BoundedContexts = ["Accounts"];

    public static readonly (string Context, string Slice)[] Slices =
    [
        ("Accounts", "Auth"),
        ("Accounts", ContextShared),
    ];
}
