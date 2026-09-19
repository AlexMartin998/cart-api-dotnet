using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using static CartAPI.Tests.Architecture.ArchitectureFixture;

namespace CartAPI.Tests.Architecture;

public sealed class LayerRulesTests
{
    [Fact]
    public void Domain_does_not_depend_on_frameworks() =>
        Types().That().ResideInNamespaceMatching(AnyDomain)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching($"({EntityFramework})|({AspNetCore})")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    [Fact]
    public void Domain_does_not_depend_on_outer_layers() =>
        Types().That().ResideInNamespaceMatching(AnyDomain)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(
                $"({AnyApplication})|({AnyInfrastructure})|({PersistenceNamespace})|({HostNamespace})")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    [Fact]
    public void Domain_has_no_validation_attributes() =>
        Types().That().ResideInNamespaceMatching(AnyDomain)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(@"^System\.ComponentModel\.DataAnnotations(\.|$)")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    [Fact]
    public void Application_does_not_depend_on_infrastructure_or_aspnet() =>
        Types().That().ResideInNamespaceMatching(AnyApplication)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching($"({AnyInfrastructure})|({HostNamespace})|({AspNetCore})")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    [Fact]
    public void Command_handlers_do_not_use_the_DbContext() =>
        Classes().That().HaveNameEndingWith("CommandHandler")
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching($"({PersistenceNamespace})|({EntityFramework})")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    [Fact]
    public void Shared_does_not_depend_on_features_persistence_or_host() =>
        Types().That().ResideInNamespaceMatching(SharedNamespace)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching($"({FeaturesNamespace})|({PersistenceNamespace})|({HostNamespace})")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    [Fact]
    public void Http_adapters_do_not_use_persistence() =>
        Types().That().ResideInNamespaceMatching(AnyHttpAdapter)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching($"({PersistenceNamespace})|({EntityFramework})")
            .AndShould().NotDependOnAnyTypesThat().HaveNameEndingWith("Repository")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    [Fact]
    public void Contracts_expose_only_primitives() =>
        Types().That().ResideInNamespaceMatching(AnyContracts)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching($"({AnyDomain})|({EntityFramework})|({AspNetCore})")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    // Native validation skips internal types.
    [Fact]
    public void Feature_infrastructure_is_internal() =>
        Types().That().ResideInNamespaceMatching(AnyFeatureInfrastructure)
            .And().DoNotHaveNameEndingWith("Body")
            .And().DoNotHaveNameEndingWith("Parameters")
            .Should().NotBePublic()
            .WithoutRequiringPositiveResults()
            .Check(Api);
}
