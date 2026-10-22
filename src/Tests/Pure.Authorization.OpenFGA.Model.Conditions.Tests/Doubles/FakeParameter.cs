using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeParameter : IConditionParameter
{
    public FakeParameter(string name, string type)
        : this(name, new FakeParameterType(type)) { }

    public FakeParameter(string name, IConditionParameterType type)
    {
        Name = new String(name);
        Type = type;
    }

    public IString Name { get; }

    public IConditionParameterType Type { get; }
}
