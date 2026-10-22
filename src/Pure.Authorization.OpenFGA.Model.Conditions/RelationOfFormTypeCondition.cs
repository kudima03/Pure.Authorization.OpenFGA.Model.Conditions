using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the object type a subject form refers to (OpenFGA
/// <c>RelationReference.type</c>; for a conditional form, the type of the form it
/// conditions) defines the relation. Throws <see cref="ArgumentException"/> for an
/// <see cref="ISubjectForm"/> that is none of the known variants.
/// </summary>
internal sealed record RelationOfFormTypeCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly ISubjectForm _form;

    private readonly IString _relation;

    public RelationOfFormTypeCondition(
        IAuthorizationModel model,
        ISubjectForm form,
        IString relation
    )
    {
        _model = model;
        _form = form;
        _relation = relation;
    }

    public bool BoolValue =>
        _form switch
        {
            ITypeForm type => new RelationOfTypeCondition(
                _model,
                type.Type,
                _relation
            ).BoolValue,
            ISubjectSetForm subjectSet => new RelationOfTypeCondition(
                _model,
                subjectSet.Type,
                _relation
            ).BoolValue,
            IWildcardForm wildcard => new RelationOfTypeCondition(
                _model,
                wildcard.Type,
                _relation
            ).BoolValue,
            IConditionalForm conditional => new RelationOfFormTypeCondition(
                _model,
                conditional.Form,
                _relation
            ).BoolValue,
            _ => throw new ArgumentException(),
        };
}
