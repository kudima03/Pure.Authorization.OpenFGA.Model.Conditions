using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeFrom : IRelationFrom
{
    public FakeFrom(string relation, string tupleset)
    {
        Relation = new String(relation);
        Tupleset = new String(tupleset);
    }

    public IString Relation { get; }

    public IString Tupleset { get; }
}
