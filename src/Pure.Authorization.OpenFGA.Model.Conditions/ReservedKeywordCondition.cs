using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool.Operations;
using String = Pure.Primitives.String.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a text is <c>self</c> or <c>this</c>, the keywords OpenFGA reserves and
/// rejects as type and relation names (<c>openfga/openfga</c>
/// <c>pkg/typesystem/typesystem.go</c>, <c>validateNames</c>).
/// </summary>
internal sealed record ReservedKeywordCondition : IBool
{
    private readonly IString _text;

    public ReservedKeywordCondition(IString text)
    {
        _text = text;
    }

    public bool BoolValue =>
        new Or(
            new StringEqualCondition(_text, new String("self")),
            new StringEqualCondition(_text, new String("this"))
        ).BoolValue;
}
