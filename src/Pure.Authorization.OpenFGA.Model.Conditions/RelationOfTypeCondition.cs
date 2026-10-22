using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the model defines a type with the given name and that type defines a
/// relation with the given name.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>GetRelation</c>, the lookup behind OpenFGA's <c>ErrObjectTypeUndefined</c> and
/// <c>ErrRelationUndefined</c>.
/// </remarks>
public sealed record RelationOfTypeCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly IString _type;

    private readonly IString _relation;

    public RelationOfTypeCondition(
        IAuthorizationModel model,
        IString type,
        IString relation
    )
    {
        _model = model;
        _type = type;
        _relation = relation;
    }

    public bool BoolValue => new RelationDefinitions(_model, _type, _relation).Any();
}
