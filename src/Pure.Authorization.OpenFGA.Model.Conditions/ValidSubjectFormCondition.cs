using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a subject form (one type restriction of a directly assignable relation) is
/// consistent with the model:
/// <list type="bullet">
/// <item>an <see cref="ITypeForm"/> or <see cref="IWildcardForm"/> names a type the model
/// defines;</item>
/// <item>an <see cref="ISubjectSetForm"/> names a type the model defines and a relation
/// of that type;</item>
/// <item>an <see cref="IConditionalForm"/> conditions a form that is not itself
/// conditional and is valid, and names a condition the model defines.</item>
/// </list>
/// Throws <see cref="ArgumentException"/> for an <see cref="ISubjectForm"/> that is none
/// of the known variants.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>validateTypeRestrictions</c>. A <c>RelationReference</c> carries at most one
/// <c>condition</c> (<c>openfga/api</c> <c>openfga/v1/authzmodel.proto</c>), so a
/// conditional form of a conditional form has no OpenFGA counterpart.
/// </remarks>
public sealed record ValidSubjectFormCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly ISubjectForm _form;

    public ValidSubjectFormCondition(IAuthorizationModel model, ISubjectForm form)
    {
        _model = model;
        _form = form;
    }

    public bool BoolValue =>
        _form switch
        {
            ITypeForm type => new KnownTypeCondition(_model, type.Type).BoolValue,
            ISubjectSetForm subjectSet => new RelationOfTypeCondition(
                _model,
                subjectSet.Type,
                subjectSet.Relation
            ).BoolValue,
            IWildcardForm wildcard => new KnownTypeCondition(
                _model,
                wildcard.Type
            ).BoolValue,
            IConditionalForm { Form: IConditionalForm } => false,
            IConditionalForm conditional => new And(
                new ValidSubjectFormCondition(_model, conditional.Form),
                new KnownConditionCondition(_model, conditional.Condition)
            ).BoolValue,
            _ => throw new ArgumentException(),
        };
}
