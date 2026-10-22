using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Number;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a text is a valid OpenFGA condition name: 1 to 50 code points, none of which
/// is <c>:</c>, <c>#</c>, <c>@</c> or whitespace (<c>\t</c>, <c>\n</c>, <c>\f</c>,
/// <c>\r</c> or space).
/// </summary>
/// <remarks>
/// Sources: <c>openfga/api</c> <c>openfga/v1/authzmodel.proto</c>,
/// <c>AuthorizationModel.conditions</c> key pattern and <c>Condition.name</c> pattern
/// <c>^[^:#@\s]{1,50}$</c>.
/// </remarks>
public sealed record ValidConditionNameCondition : IBool
{
    private readonly IString _condition;

    public ValidConditionNameCondition(IString condition)
    {
        _condition = condition;
    }

    public bool BoolValue => new IdentifierCondition(_condition, new Int(50)).BoolValue;
}
