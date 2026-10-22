using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when no two of the texts are equal (ordinal comparison, as Go map keys compare).
/// </summary>
internal sealed record UniqueTextCondition : IBool
{
    private readonly IEnumerable<IString> _texts;

    public UniqueTextCondition(IEnumerable<IString> texts)
    {
        _texts = texts;
    }

    public bool BoolValue =>
        _texts.DistinctBy(text => text.TextValue).Count() == _texts.Count();
}
