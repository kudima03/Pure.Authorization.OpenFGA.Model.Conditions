using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when every relation the rewrite of a relation refers to resolves:
/// <list type="bullet">
/// <item>an <see cref="IComputedRelation"/> names a relation of the same type, other than
/// the relation being defined;</item>
/// <item>an <see cref="IRelationFrom"/> names as its tupleset a relation of the same type
/// whose rewrite is exactly an <see cref="IDirect"/>, all of whose forms are plain types
/// (an <see cref="ITypeForm"/>, conditional or not, never a subject set or a wildcard),
/// and at least one of those types defines <see cref="IRelationFrom.Relation"/>;</item>
/// <item>the operands of an <see cref="IUnion"/>, <see cref="IIntersection"/> or
/// <see cref="IDifference"/> resolve, recursively.</item>
/// </list>
/// Throws <see cref="ArgumentException"/> when the rewrite holds an
/// <see cref="IRewrite"/>, or a tupleset relation an <see cref="ISubjectForm"/>, that is
/// none of the known variants.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>isUsersetRewriteValid</c> (schema 1.1 and later: the computed relation of a
/// tuple-to-userset must be defined on at least one of the tupleset's directly related
/// types) and <c>validateTypeRestrictions</c> (a tupleset relation must not be assignable
/// from a subject set or a wildcard).
/// </remarks>
public sealed record ResolvableRewriteCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly ITypeDefinition _type;

    private readonly IRelationDefinition _relation;

    public ResolvableRewriteCondition(
        IAuthorizationModel model,
        ITypeDefinition type,
        IRelationDefinition relation
    )
    {
        _model = model;
        _type = type;
        _relation = relation;
    }

    public bool BoolValue =>
        _relation.Rewrite switch
        {
            IDirect => true,
            IComputedRelation computed => new And(
                new Not(new StringEqualCondition(computed.Relation, _relation.Name)),
                new NotEmptyCondition<IRelationDefinition>(
                    new RelationDefinitions(_type, computed.Relation)
                )
            ).BoolValue,
            IRelationFrom from => new AnyCondition<IRelationDefinition>(
                new RelationDefinitions(_type, from.Tupleset),
                tupleset => new TuplesetCondition(_model, tupleset.Rewrite, from)
            ).BoolValue,
            IUnion union => new AllCondition<IRewrite>(union.Children, Operand).BoolValue,
            IIntersection intersection => new AllCondition<IRewrite>(
                intersection.Children,
                Operand
            ).BoolValue,
            IDifference difference => new And(
                Operand(difference.Base),
                Operand(difference.Subtract)
            ).BoolValue,
            _ => throw new ArgumentException(),
        };

    private ResolvableRewriteCondition Operand(IRewrite operand)
    {
        return new ResolvableRewriteCondition(
            _model,
            _type,
            new RelationWithRewrite(_relation, operand)
        );
    }
}
