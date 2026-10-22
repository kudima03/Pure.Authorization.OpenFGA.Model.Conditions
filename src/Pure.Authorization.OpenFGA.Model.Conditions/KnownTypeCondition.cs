using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the model defines a type with the given name.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>GetTypeDefinition</c>, the lookup behind OpenFGA's <c>type ... not found</c>
/// errors.
/// </remarks>
public sealed record KnownTypeCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly IString _type;

    public KnownTypeCondition(IAuthorizationModel model, IString type)
    {
        _model = model;
        _type = type;
    }

    public bool BoolValue => new TypeDefinitions(_model, _type).Any();
}
