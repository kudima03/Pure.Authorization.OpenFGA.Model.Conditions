using System.Text.Json.Nodes;
using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Authorization.OpenFGA.Model.Abstractions.Subject;
using Pure.Authorization.OpenFGA.Model.Abstractions.Tuple;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeConditionalTuple : IConditionalTuple
{
    private readonly ITuple _tuple;

    public FakeConditionalTuple(ITuple tuple, string condition)
        : this(tuple, new FakeRelationshipCondition(condition, new JsonObject())) { }

    public FakeConditionalTuple(ITuple tuple, IRelationshipCondition condition)
    {
        _tuple = tuple;
        Condition = condition;
    }

    public ISubject Subject => _tuple.Subject;

    public IString Relation => _tuple.Relation;

    public IObject Object => _tuple.Object;

    public IRelationshipCondition Condition { get; }
}
