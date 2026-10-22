using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// The number of Unicode code points in a text. OpenFGA validates its identifiers with Go
/// regular expressions, whose length bounds such as <c>{1,50}</c> count code points (runes),
/// not UTF-16 code units.
/// </summary>
internal sealed record RuneLength : INumber<int>
{
    private readonly IString _text;

    public RuneLength(IString text)
    {
        _text = text;
    }

    public int NumberValue => _text.TextValue.EnumerateRunes().Count();
}
