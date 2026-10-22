using System.Text.Json.Nodes;
using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Tuple;
using Pure.Primitives.Abstractions.Bool;
using String = Pure.Primitives.String.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the context of a tuple's condition is a JSON object whose every key is a
/// parameter of the condition the model defines under that name.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>internal/validation/validation.go</c>,
/// <c>validateCondition</c> (<c>found invalid context parameter</c>), and
/// <c>internal/condition/condition.go</c>, <c>CastContextToTypedParameters</c>.
/// </remarks>
internal sealed record ConditionContextCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly IRelationshipCondition _condition;

    public ConditionContextCondition(
        IAuthorizationModel model,
        IRelationshipCondition condition
    )
    {
        _model = model;
        _condition = condition;
    }

    public bool BoolValue =>
        _condition.Context is JsonObject context
        && context.All(property =>
            new AnyCondition<IConditionParameter>(
                _model
                    .Conditions.Where(condition =>
                        new StringEqualCondition(
                            condition.Name,
                            _condition.Name
                        ).BoolValue
                    )
                    .SelectMany(condition => condition.Parameters),
                parameter => new StringEqualCondition(
                    parameter.Name,
                    new String(property.Key)
                )
            ).BoolValue
        );
}
