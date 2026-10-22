using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when no two relations of the type definition share a name.
/// </summary>
/// <remarks>
/// Source: <c>openfga/api</c> <c>openfga/v1/authzmodel.proto</c>,
/// <c>TypeDefinition.relations</c>, a <c>map&lt;string, Userset&gt;</c> keyed by relation
/// name, so OpenFGA cannot represent a duplicate.
/// </remarks>
public sealed record UniqueRelationNamesCondition : IBool
{
    private readonly ITypeDefinition _type;

    public UniqueRelationNamesCondition(ITypeDefinition type)
    {
        _type = type;
    }

    public bool BoolValue =>
        new UniqueTextCondition(
            _type.Relations.Select(relation => relation.Name)
        ).BoolValue;
}
