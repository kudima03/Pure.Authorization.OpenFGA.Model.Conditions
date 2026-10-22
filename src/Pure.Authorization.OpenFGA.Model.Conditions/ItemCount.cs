using Pure.Primitives.Abstractions.Number;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// The number of items in a sequence, counted when read.
/// </summary>
internal sealed record ItemCount<T> : INumber<int>
{
    private readonly IEnumerable<T> _items;

    public ItemCount(IEnumerable<T> items)
    {
        _items = items;
    }

    public int NumberValue => _items.Count();
}
