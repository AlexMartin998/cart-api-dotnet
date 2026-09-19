using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using static CartAPI.Tests.Architecture.ArchitectureFixture;

namespace CartAPI.Tests.Architecture;

public sealed class ContextRulesTests
{
    // A Theory without data fails, and one context yields no pairs.
    [Fact]
    public void A_context_only_sees_the_Contracts_of_another()
    {
        foreach (var from in BoundedContexts)
        {
            foreach (var to in BoundedContexts.Where(c => c != from))
            {
                Types().That().ResideInNamespaceMatching($@"^CartAPI\.Features\.{from}(\.|$)")
                    .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching($@"^CartAPI\.Features\.{to}\.(?!Contracts(\.|$))")
                    .WithoutRequiringPositiveResults()
                    .Check(Api);
            }
        }
    }

    public static TheoryData<string, string, string> SlicePairs()
    {
        var data = new TheoryData<string, string, string>();
        foreach (var (context, from) in Slices)
        {
            foreach (var (_, to) in Slices.Where(s => s.Context == context && s.Slice != from && s.Slice != ContextShared))
                data.Add(context, from, to);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(SlicePairs))]
    public void A_slice_only_sees_the_Domain_of_its_sibling_slices(string context, string from, string to) =>
        Types().That().ResideInNamespaceMatching($@"^CartAPI\.Features\.{context}\.{from}(\.|$)")
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(
                $@"^CartAPI\.Features\.{context}\.{to}\.(Application|Infrastructure)(\.|$)")
            .WithoutRequiringPositiveResults()
            .Check(Api);

    [Fact]
    public void Feature_namespaces_follow_context_slice_layer()
    {
        var slices = Slices.Select(s => $"{s.Context}.{s.Slice}").ToHashSet();
        var offenders = Api.Types
            .Select(t => t.Namespace.FullName)
            .Where(ns => ns.StartsWith("CartAPI.Features.", StringComparison.Ordinal))
            .Distinct()
            .Where(ns => !IsAllowed(ns["CartAPI.Features.".Length..].Split('.'), slices))
            .ToList();

        Assert.Empty(offenders);
    }

    private static bool IsAllowed(string[] parts, HashSet<string> slices) => parts switch
    {
        [var context] => BoundedContexts.Contains(context),
        [_, "Contracts", ..] => true,
        [var context, var slice, "Domain" or "Application" or "Infrastructure", ..] => slices.Contains($"{context}.{slice}"),
        _ => false,
    };
}
