using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeParameterType : IConditionParameterType
{
    public FakeParameterType(
        string name,
        params IEnumerable<IConditionParameterType> genericTypes
    )
    {
        Name = new String(name);
        GenericTypes = genericTypes;
    }

    public IString Name { get; }

    public IEnumerable<IConditionParameterType> GenericTypes { get; }
}
