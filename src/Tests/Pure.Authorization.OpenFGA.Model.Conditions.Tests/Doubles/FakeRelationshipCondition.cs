using System.Text.Json.Nodes;
using Pure.Authorization.OpenFGA.Model.Abstractions.Tuple;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeRelationshipCondition : IRelationshipCondition
{
    public FakeRelationshipCondition(string name, JsonNode context)
    {
        Name = new String(name);
        Context = context;
    }

    public IString Name { get; }

    public JsonNode Context { get; }
}
