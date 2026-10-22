using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.String.Operations;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a part of a tuple field (the type, id or relation in <c>type:id#relation</c>)
/// is not empty and contains none of the given separators and no control character. This
/// is the per-part reading of the character scanners in <c>openfga/openfga</c>
/// <c>pkg/tuple/tuple.go</c> (<c>IsValidObject</c>, <c>IsValidUserset</c>).
/// </summary>
internal sealed record TupleFieldPartCondition : IBool
{
    private readonly IString _part;

    private readonly IString _forbidden;

    public TupleFieldPartCondition(IString part, IString forbidden)
    {
        _part = part;
        _forbidden = forbidden;
    }

    public bool BoolValue =>
        new And(
            new Not(new EmptyTextCondition(_part)),
            new Not(new TextContainingAnyCharCondition(_part, _forbidden)),
            new Not(new TextContainingControlCharCondition(_part))
        ).BoolValue;
}
