using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record ModelWithSchemaVersion : IAuthorizationModel
{
    private readonly IAuthorizationModel _origin;

    public ModelWithSchemaVersion(IAuthorizationModel origin, string schemaVersion)
    {
        _origin = origin;
        SchemaVersion = new String(schemaVersion);
    }

    public IString SchemaVersion { get; }

    public IEnumerable<ITypeDefinition> Types => _origin.Types;

    public IEnumerable<ICondition> Conditions => _origin.Conditions;
}
