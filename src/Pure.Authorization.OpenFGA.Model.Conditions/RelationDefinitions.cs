using System.Collections;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// The relation definitions named <c>relation</c> on the type definitions named
/// <c>type</c>.
/// </summary>
internal sealed record RelationDefinitions : IEnumerable<IRelationDefinition>
{
    private readonly IEnumerable<ITypeDefinition> _types;

    private readonly IString _relation;

    public RelationDefinitions(IAuthorizationModel model, IString type, IString relation)
        : this(new TypeDefinitions(model, type), relation) { }

    public RelationDefinitions(ITypeDefinition type, IString relation)
        : this([type], relation) { }

    private RelationDefinitions(IEnumerable<ITypeDefinition> types, IString relation)
    {
        _types = types;
        _relation = relation;
    }

    public IEnumerator<IRelationDefinition> GetEnumerator()
    {
        return _types
            .SelectMany(type => type.Relations)
            .Where(definition =>
                new StringEqualCondition(definition.Name, _relation).BoolValue
            )
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
