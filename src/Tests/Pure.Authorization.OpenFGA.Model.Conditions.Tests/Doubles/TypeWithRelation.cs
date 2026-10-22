using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

/// <summary>
/// The type with the relation of the same name replaced, or the relation appended if the
/// type has none of that name.
/// </summary>
internal sealed record TypeWithRelation : ITypeDefinition
{
    private readonly ITypeDefinition _origin;

    private readonly IRelationDefinition _relation;

    public TypeWithRelation(ITypeDefinition origin, IRelationDefinition relation)
    {
        _origin = origin;
        _relation = relation;
    }

    public IString Name => _origin.Name;

    public IEnumerable<IRelationDefinition> Relations =>
        _origin.Relations.Any(relation =>
            relation.Name.TextValue == _relation.Name.TextValue
        )
            ? _origin.Relations.Select(relation =>
                relation.Name.TextValue == _relation.Name.TextValue ? _relation : relation
            )
            : _origin.Relations.Append(_relation);
}
