using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Authorization.OpenFGA.Model.Abstractions.Subject;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.Bool;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a subject form admits a subject, ignoring conditions: a type form admits an
/// object of that type, a subject-set form a subject set of the same type and relation,
/// and a wildcard form a typed wildcard of that type. A conditional form admits nothing
/// here; its condition is matched by the caller. Throws <see cref="ArgumentException"/>
/// for an <see cref="ISubjectForm"/> that is none of the known variants.
/// </summary>
internal sealed record SubjectFormAdmitsCondition : IBool
{
    private readonly ISubjectForm _form;

    private readonly ISubject _subject;

    public SubjectFormAdmitsCondition(ISubjectForm form, ISubject subject)
    {
        _form = form;
        _subject = subject;
    }

    public bool BoolValue =>
        _form switch
        {
            ITypeForm type => _subject is IObject @object
                && new StringEqualCondition(type.Type, @object.Type).BoolValue,
            ISubjectSetForm subjectSetForm => _subject is ISubjectSet subjectSet
                && new StringEqualCondition(
                    subjectSetForm.Type,
                    subjectSet.Object.Type
                ).BoolValue
                && new StringEqualCondition(
                    subjectSetForm.Relation,
                    subjectSet.Relation
                ).BoolValue,
            IWildcardForm wildcardForm => _subject is ITypedWildcard wildcard
                && new StringEqualCondition(wildcardForm.Type, wildcard.Type).BoolValue,
            IConditionalForm => false,
            _ => throw new ArgumentException(),
        };
}
