using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a subject form names a plain object type: an <see cref="ITypeForm"/>, or an
/// <see cref="IConditionalForm"/> of one. OpenFGA's <c>RelationReference</c> then sets
/// neither <c>relation</c> nor <c>wildcard</c>. Throws <see cref="ArgumentException"/> for
/// an <see cref="ISubjectForm"/> that is none of the known variants.
/// </summary>
internal sealed record PlainTypeFormCondition : IBool
{
    private readonly ISubjectForm _form;

    public PlainTypeFormCondition(ISubjectForm form)
    {
        _form = form;
    }

    public bool BoolValue =>
        _form switch
        {
            ITypeForm => true,
            ISubjectSetForm or IWildcardForm => false,
            IConditionalForm conditional => new PlainTypeFormCondition(
                conditional.Form
            ).BoolValue,
            _ => throw new ArgumentException(),
        };
}
