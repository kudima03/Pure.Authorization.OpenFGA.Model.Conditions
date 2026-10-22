using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the condition holds for at least one item; does not hold for no items.
/// Unlike <c>Pure.Primitives.Bool.Operations.Or</c>, it accepts an empty sequence.
/// </summary>
internal sealed record AnyCondition<T> : IBool
{
    private readonly IEnumerable<T> _items;

    private readonly Func<T, IBool> _condition;

    public AnyCondition(IEnumerable<T> items, Func<T, IBool> condition)
    {
        _items = items;
        _condition = condition;
    }

    public bool BoolValue => _items.Any(item => _condition(item).BoolValue);
}
