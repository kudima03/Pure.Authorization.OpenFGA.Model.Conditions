using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a text is a valid OpenFGA type name: 1 to 254 code points, none of which is
/// <c>:</c>, <c>#</c>, <c>@</c> or whitespace (<c>\t</c>, <c>\n</c>, <c>\f</c>, <c>\r</c>
/// or space), and neither of the reserved keywords <c>self</c> and <c>this</c>.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/api</c> <c>openfga/v1/authzmodel.proto</c>,
/// <c>TypeDefinition.type</c> pattern <c>^[^:#@\s]{1,254}$</c>; <c>openfga/openfga</c>
/// <c>pkg/typesystem/typesystem.go</c>, <c>validateNames</c>.
/// </remarks>
public sealed record ValidTypeNameCondition : IBool
{
    private readonly IString _type;

    public ValidTypeNameCondition(IString type)
    {
        _type = type;
    }

    public bool BoolValue =>
        new And(
            new IdentifierCondition(_type, new Int(254)),
            new Not(new ReservedKeywordCondition(_type))
        ).BoolValue;
}
