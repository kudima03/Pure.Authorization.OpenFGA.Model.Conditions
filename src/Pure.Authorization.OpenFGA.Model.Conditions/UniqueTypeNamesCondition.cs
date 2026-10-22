using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when no two type definitions of the model share a name.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>containsDuplicateType</c> (<c>ErrDuplicateTypes</c>).
/// </remarks>
public sealed record UniqueTypeNamesCondition : IBool
{
    private readonly IAuthorizationModel _model;

    public UniqueTypeNamesCondition(IAuthorizationModel model)
    {
        _model = model;
    }

    public bool BoolValue =>
        new UniqueTextCondition(_model.Types.Select(type => type.Name)).BoolValue;
}
