using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

/// <summary>
/// The model with one relation of one of its types replaced or added.
/// </summary>
internal sealed record ModelWithRelation : IAuthorizationModel
{
    private readonly IAuthorizationModel _model;

    public ModelWithRelation(
        IAuthorizationModel origin,
        string type,
        IRelationDefinition relation
    )
    {
        _model = new ModelWithType(
            origin,
            new TypeWithRelation(new ModelType(origin, type), relation)
        );
    }

    public IString SchemaVersion => _model.SchemaVersion;

    public IEnumerable<ITypeDefinition> Types => _model.Types;

    public IEnumerable<ICondition> Conditions => _model.Conditions;
}
