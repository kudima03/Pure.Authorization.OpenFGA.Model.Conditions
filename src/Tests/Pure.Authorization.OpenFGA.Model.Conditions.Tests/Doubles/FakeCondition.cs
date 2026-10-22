using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeCondition : ICondition
{
    public FakeCondition(string name, params IEnumerable<IConditionParameter> parameters)
        : this(name, "true", parameters) { }

    public FakeCondition(
        string name,
        string expression,
        IEnumerable<IConditionParameter> parameters
    )
    {
        Name = new String(name);
        Expression = new String(expression);
        Parameters = parameters;
    }

    public IString Name { get; }

    public IString Expression { get; }

    public IEnumerable<IConditionParameter> Parameters { get; }
}
