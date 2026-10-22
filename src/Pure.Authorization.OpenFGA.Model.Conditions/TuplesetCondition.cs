using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the rewrite of a tupleset relation can serve an <see cref="IRelationFrom"/>:
/// it is exactly an <see cref="IDirect"/>, at least one of its forms refers to a type that
/// defines the looked-up relation, and every form is a plain type.
/// </summary>
internal sealed record TuplesetCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly IRewrite _tupleset;

    private readonly IRelationFrom _from;

    public TuplesetCondition(
        IAuthorizationModel model,
        IRewrite tupleset,
        IRelationFrom from
    )
    {
        _model = model;
        _tupleset = tupleset;
        _from = from;
    }

    public bool BoolValue =>
        _tupleset switch
        {
            IDirect direct => new And(
                new AnyCondition<ISubjectForm>(
                    direct.Forms,
                    form => new RelationOfFormTypeCondition(_model, form, _from.Relation)
                ),
                new AllCondition<ISubjectForm>(
                    direct.Forms,
                    form => new PlainTypeFormCondition(form)
                )
            ).BoolValue,
            _ => false,
        };
}
