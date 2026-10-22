using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Char.Operations;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a text contains a control character, as Go's <c>unicode.IsControl</c> (and
/// .NET's <see cref="char.IsControl(char)"/>) define it: U+0000 to U+001F and U+007F to
/// U+009F.
/// </summary>
internal sealed record TextContainingControlCharCondition : IBool
{
    private readonly IString _text;

    public TextContainingControlCharCondition(IString text)
    {
        _text = text;
    }

    public bool BoolValue =>
        _text.Any(symbol => new IsControlCondition(symbol).BoolValue);
}
