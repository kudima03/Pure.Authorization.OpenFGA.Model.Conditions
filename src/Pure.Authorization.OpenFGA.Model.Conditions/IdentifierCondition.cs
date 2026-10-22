using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number.Operations;
using Pure.Primitives.String.Operations;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a text matches OpenFGA's identifier pattern <c>^[^:#@\s]{1,N}$</c>: one to
/// <c>N</c> code points, none of which is <c>:</c>, <c>#</c>, <c>@</c> or whitespace. The
/// pattern is a Go regular expression, in which <c>\s</c> is exactly
/// <c>[\t\n\f\r ]</c>.
/// </summary>
internal sealed record IdentifierCondition : IBool
{
    private readonly IString _text;

    private readonly INumber<int> _maxLength;

    public IdentifierCondition(IString text, INumber<int> maxLength)
    {
        _text = text;
        _maxLength = maxLength;
    }

    public bool BoolValue =>
        new And(
            new Not(new EmptyTextCondition(_text)),
            new Not(
                new TextContainingAnyCharCondition(_text, new String(":#@\t\n\f\r "))
            ),
            new LessThanOrEqualCondition<int>(new RuneLength(_text), _maxLength)
        ).BoolValue;
}
