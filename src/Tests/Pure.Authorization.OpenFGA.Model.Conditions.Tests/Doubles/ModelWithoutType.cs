using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record ModelWithoutType : IAuthorizationModel
{
    private readonly IAuthorizationModel _origin;

    private readonly string _type;

    public ModelWithoutType(IAuthorizationModel origin, string type)
    {
        _origin = origin;
        _type = type;
    }

    public IString SchemaVersion => _origin.SchemaVersion;

    public IEnumerable<ITypeDefinition> Types =>
        _origin.Types.Where(type => type.Name.TextValue != _type);

    public IEnumerable<ICondition> Conditions => _origin.Conditions;
}
