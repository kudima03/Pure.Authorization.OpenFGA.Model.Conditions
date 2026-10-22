using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeModel : IAuthorizationModel
{
    public FakeModel(params IEnumerable<ITypeDefinition> types)
        : this(types, []) { }

    public FakeModel(
        IEnumerable<ITypeDefinition> types,
        IEnumerable<ICondition> conditions
    )
        : this(new String("1.1"), types, conditions) { }

    public FakeModel(
        IString schemaVersion,
        IEnumerable<ITypeDefinition> types,
        IEnumerable<ICondition> conditions
    )
    {
        SchemaVersion = schemaVersion;
        Types = types;
        Conditions = conditions;
    }

    public IString SchemaVersion { get; }

    public IEnumerable<ITypeDefinition> Types { get; }

    public IEnumerable<ICondition> Conditions { get; }
}
