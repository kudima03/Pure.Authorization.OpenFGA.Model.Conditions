using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Authorization.OpenFGA.Model.Abstractions.Tuple;
using Pure.Primitives.Abstractions.Bool;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a type restriction admits a tuple: a conditional form admits a conditional
/// tuple with the same condition name whose subject the conditioned form admits; any
/// other form admits a tuple without a condition whose subject it admits.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>internal/validation/validation.go</c>,
/// <c>validateTypeRestrictions</c>, <c>validateCondition</c> and
/// <c>restrictionFacetMatches</c>.
/// </remarks>
internal sealed record FormAdmitsTupleCondition : IBool
{
    private readonly ISubjectForm _form;

    private readonly ITuple _tuple;

    public FormAdmitsTupleCondition(ISubjectForm form, ITuple tuple)
    {
        _form = form;
        _tuple = tuple;
    }

    public bool BoolValue =>
        _form switch
        {
            IConditionalForm conditional => _tuple is IConditionalTuple conditionalTuple
                && new StringEqualCondition(
                    conditional.Condition,
                    conditionalTuple.Condition.Name
                ).BoolValue
                && new SubjectFormAdmitsCondition(
                    conditional.Form,
                    _tuple.Subject
                ).BoolValue,
            _ => new SubjectFormAdmitsCondition(_form, _tuple.Subject).BoolValue
                && _tuple is not IConditionalTuple,
        };
}
