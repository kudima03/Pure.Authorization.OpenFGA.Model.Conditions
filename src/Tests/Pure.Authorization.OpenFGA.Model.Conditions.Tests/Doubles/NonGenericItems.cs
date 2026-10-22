using System.Collections;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

/// <summary>
/// The items of a sequence read through its non-generic <see cref="IEnumerable"/>
/// enumerator.
/// </summary>
internal sealed record NonGenericItems : IEnumerable<object?>
{
    private readonly IEnumerable _items;

    public NonGenericItems(IEnumerable items)
    {
        _items = items;
    }

    public IEnumerator<object?> GetEnumerator()
    {
        IEnumerator enumerator = _items.GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
