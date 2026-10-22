using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number;
using Pure.Primitives.Number.Operations;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the parameters of a condition are valid: at most 25 of them, with unique
/// names, each name 1 to 50 code points with no <c>:</c>, <c>#</c>, <c>@</c> or whitespace
/// (<c>\t</c>, <c>\n</c>, <c>\f</c>, <c>\r</c> or space), and each type a
/// <see cref="ValidConditionParameterTypeCondition"/>.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/api</c> <c>openfga/v1/authzmodel.proto</c>,
/// <c>Condition.parameters</c> (a map keyed by parameter name, <c>max_pairs: 25</c>, key
/// pattern <c>^[^:#@\s]{1,50}$</c>); <c>openfga/openfga</c>
/// <c>internal/condition/types/encoding.go</c>, <c>DecodeParameterType</c>.
/// </remarks>
public sealed record ValidConditionParametersCondition : IBool
{
    private readonly ICondition _condition;

    public ValidConditionParametersCondition(ICondition condition)
    {
        _condition = condition;
    }

    public bool BoolValue =>
        new And(
            new LessThanOrEqualCondition<int>(
                new ItemCount<IConditionParameter>(_condition.Parameters),
                new Int(25)
            ),
            new UniqueTextCondition(
                _condition.Parameters.Select(parameter => parameter.Name)
            ),
            new AllCondition<IConditionParameter>(
                _condition.Parameters,
                parameter => new And(
                    new IdentifierCondition(parameter.Name, new Int(50)),
                    new ValidConditionParameterTypeCondition(parameter.Type)
                )
            )
        ).BoolValue;
}
