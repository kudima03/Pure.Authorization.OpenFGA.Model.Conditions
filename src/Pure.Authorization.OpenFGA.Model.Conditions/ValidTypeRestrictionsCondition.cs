using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the type restrictions of a relation (the forms of every
/// <see cref="IDirect"/> in its rewrite) are consistent with the model: a relation whose
/// rewrite contains an <see cref="IDirect"/> has at least one form, and every form is a
/// <see cref="ValidSubjectFormCondition"/>. Throws <see cref="ArgumentException"/> when the
/// rewrite or a form is none of the known variants.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>validateTypeRestrictions</c> (<c>AssignableRelationError</c> and the per-reference
/// checks). The tupleset rule of the same function is checked by
/// <see cref="ResolvableRewriteCondition"/>.
/// </remarks>
public sealed record ValidTypeRestrictionsCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly IRelationDefinition _relation;

    public ValidTypeRestrictionsCondition(
        IAuthorizationModel model,
        IRelationDefinition relation
    )
    {
        _model = model;
        _relation = relation;
    }

    public bool BoolValue
    {
        get
        {
            IEnumerable<ISubjectForm> forms = new TypeRestrictions(_relation);

            return (
                    !new RewriteNodes(_relation.Rewrite).OfType<IDirect>().Any()
                    || forms.Any()
                )
                && new AllCondition<ISubjectForm>(
                    forms,
                    form => new ValidSubjectFormCondition(_model, form)
                ).BoolValue;
        }
    }
}
