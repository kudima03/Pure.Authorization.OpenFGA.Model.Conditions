using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeComputed : IComputedRelation
{
    public FakeComputed(string relation)
    {
        Relation = new String(relation);
    }

    public IString Relation { get; }
}
