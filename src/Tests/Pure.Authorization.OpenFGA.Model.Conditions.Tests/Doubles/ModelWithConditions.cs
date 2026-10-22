using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record ModelWithConditions : IAuthorizationModel
{
    private readonly IAuthorizationModel _origin;

    public ModelWithConditions(
        IAuthorizationModel origin,
        params IEnumerable<ICondition> conditions
    )
    {
        _origin = origin;
        Conditions = conditions;
    }

    public IString SchemaVersion => _origin.SchemaVersion;

    public IEnumerable<ITypeDefinition> Types => _origin.Types;

    public IEnumerable<ICondition> Conditions { get; }
}
