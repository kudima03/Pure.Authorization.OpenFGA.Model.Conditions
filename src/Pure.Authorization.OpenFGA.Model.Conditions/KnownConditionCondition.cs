using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the model defines a condition with the given name.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>validateTypeRestrictions</c> (<c>ErrNoConditionForRelation</c>);
/// <c>openfga/openfga</c> <c>internal/validation/validation.go</c>,
/// <c>validateCondition</c> (<c>undefined condition</c>).
/// </remarks>
public sealed record KnownConditionCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly IString _condition;

    public KnownConditionCondition(IAuthorizationModel model, IString condition)
    {
        _model = model;
        _condition = condition;
    }

    public bool BoolValue =>
        _model.Conditions.Any(condition =>
            new StringEqualCondition(condition.Name, _condition).BoolValue
        );
}
