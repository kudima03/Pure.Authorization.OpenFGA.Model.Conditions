using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a sequence has at least one item.
/// </summary>
internal sealed record NotEmptyCondition<T> : IBool
{
    private readonly IEnumerable<T> _items;

    public NotEmptyCondition(IEnumerable<T> items)
    {
        _items = items;
    }

    public bool BoolValue => _items.Any();
}
