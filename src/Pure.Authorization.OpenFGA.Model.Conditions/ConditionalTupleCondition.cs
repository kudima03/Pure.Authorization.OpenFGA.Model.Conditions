using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Tuple;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number;
using Pure.Primitives.Number.Operations;
using Pure.Primitives.String.Operations;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds for a tuple without a condition; for an <see cref="IConditionalTuple"/>, holds
/// when its condition name is 2 to 256 code points with no space or control character,
/// the model defines that condition, and the context only sets its parameters.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/api</c> <c>openfga/v1/openfga.proto</c>,
/// <c>RelationshipCondition.name</c> pattern <c>^[^\s]{2,256}$</c>;
/// <c>openfga/openfga</c> <c>internal/validation/validation.go</c>,
/// <c>validateCondition</c>.
/// </remarks>
internal sealed record ConditionalTupleCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly ITuple _tuple;

    public ConditionalTupleCondition(IAuthorizationModel model, ITuple tuple)
    {
        _model = model;
        _tuple = tuple;
    }

    public bool BoolValue =>
        _tuple switch
        {
            IConditionalTuple conditional => new And(
                new Not(
                    new TextContainingAnyCharCondition(
                        conditional.Condition.Name,
                        new String(" ")
                    )
                ),
                new Not(
                    new TextContainingControlCharCondition(conditional.Condition.Name)
                ),
                new LessThanOrEqualCondition<int>(
                    new Int(2),
                    new RuneLength(conditional.Condition.Name),
                    new Int(256)
                ),
                new KnownConditionCondition(_model, conditional.Condition.Name),
                new ConditionContextCondition(_model, conditional.Condition)
            ).BoolValue,
            _ => true,
        };
}
