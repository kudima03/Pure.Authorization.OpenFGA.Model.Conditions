using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Abstractions.Tuple;
using Pure.Primitives.Abstractions.Bool;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a tuple respects the tupleset restrictions: if its relation is the tupleset
/// of an <see cref="IRelationFrom"/> on the object's type, the relation is defined exactly
/// as an <see cref="IDirect"/> and the subject is an <see cref="IObject"/>, never a
/// subject set or a typed wildcard.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>internal/validation/validation.go</c>,
/// <c>validateTuplesetRestrictions</c>.
/// </remarks>
internal sealed record TuplesetSubjectCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly ITuple _tuple;

    public TuplesetSubjectCondition(IAuthorizationModel model, ITuple tuple)
    {
        _model = model;
        _tuple = tuple;
    }

    public bool BoolValue =>
        !new TypeDefinitions(_model, _tuple.Object.Type)
            .SelectMany(type => type.Relations)
            .SelectMany(relation => new RewriteNodes(relation.Rewrite))
            .OfType<IRelationFrom>()
            .Any(from =>
                new StringEqualCondition(from.Tupleset, _tuple.Relation).BoolValue
            )
        || (
            _tuple.Subject is IObject
            && new RelationDefinitions(_model, _tuple.Object.Type, _tuple.Relation).All(
                relation => relation.Rewrite is IDirect
            )
        );
}
