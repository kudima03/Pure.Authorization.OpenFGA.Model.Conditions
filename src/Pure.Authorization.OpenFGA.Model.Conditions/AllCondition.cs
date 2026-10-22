using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the condition holds for every item; holds for no items. Unlike
/// <c>Pure.Primitives.Bool.Operations.And</c>, it accepts an empty sequence.
/// </summary>
internal sealed record AllCondition<T> : IBool
{
    private readonly IEnumerable<T> _items;

    private readonly Func<T, IBool> _condition;

    public AllCondition(IEnumerable<T> items, Func<T, IBool> condition)
    {
        _items = items;
        _condition = condition;
    }

    public bool BoolValue => _items.All(item => _condition(item).BoolValue);
}
