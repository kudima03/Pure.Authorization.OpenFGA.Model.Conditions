using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

/// <summary>
/// The model with the type of the same name replaced, or the type appended if the model
/// has none of that name.
/// </summary>
internal sealed record ModelWithType : IAuthorizationModel
{
    private readonly IAuthorizationModel _origin;

    private readonly ITypeDefinition _type;

    public ModelWithType(IAuthorizationModel origin, ITypeDefinition type)
    {
        _origin = origin;
        _type = type;
    }

    public IString SchemaVersion => _origin.SchemaVersion;

    public IEnumerable<ITypeDefinition> Types =>
        _origin.Types.Any(type => type.Name.TextValue == _type.Name.TextValue)
            ? _origin.Types.Select(type =>
                type.Name.TextValue == _type.Name.TextValue ? _type : type
            )
            : _origin.Types.Append(_type);

    public IEnumerable<ICondition> Conditions => _origin.Conditions;
}
