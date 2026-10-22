using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when no two conditions of the model share a name.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/api</c> <c>openfga/v1/authzmodel.proto</c>,
/// <c>AuthorizationModel.conditions</c>, a <c>map&lt;string, Condition&gt;</c> keyed by
/// condition name; <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>validateConditions</c> (each key must equal its condition's name).
/// </remarks>
public sealed record UniqueConditionNamesCondition : IBool
{
    private readonly IAuthorizationModel _model;

    public UniqueConditionNamesCondition(IAuthorizationModel model)
    {
        _model = model;
    }

    public bool BoolValue =>
        new UniqueTextCondition(
            _model.Conditions.Select(condition => condition.Name)
        ).BoolValue;
}
