using System.Collections;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// The type definitions of a model named <c>type</c>.
/// </summary>
internal sealed record TypeDefinitions : IEnumerable<ITypeDefinition>
{
    private readonly IAuthorizationModel _model;

    private readonly IString _type;

    public TypeDefinitions(IAuthorizationModel model, IString type)
    {
        _model = model;
        _type = type;
    }

    public IEnumerator<ITypeDefinition> GetEnumerator()
    {
        return _model
            .Types.Where(definition =>
                new StringEqualCondition(definition.Name, _type).BoolValue
            )
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
