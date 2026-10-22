using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Authorization.OpenFGA.Model.Abstractions.Subject;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Authorization.OpenFGA.Model.Abstractions.Tuple;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when OpenFGA would accept the tuple for a write against the model:
/// <list type="bullet">
/// <item>the object is a <see cref="ValidObjectCondition"/>, the relation is 1 to 50 code
/// points with no <c>:</c>, <c>#</c>, <c>@</c>, whitespace or control character, and the
/// subject is a <see cref="ValidSubjectCondition"/>;</item>
/// <item>the model defines the object's type and the relation on it, the subject's type
/// and, for an <see cref="ISubjectSet"/>, its relation;</item>
/// <item>if the relation is the tupleset of an <see cref="Abstractions.Rewrite.IRelationFrom"/>
/// on the object's type, it is defined exactly as an
/// <see cref="Abstractions.Rewrite.IDirect"/> and the subject is an
/// <see cref="IObject"/>;</item>
/// <item>a type restriction of the relation admits the subject: a type form for an
/// <see cref="IObject"/> of its type, a subject-set form for an <see cref="ISubjectSet"/>
/// of the same type and relation, a wildcard form for an <see cref="ITypedWildcard"/> of
/// its type. A tuple without a condition needs such a form without a condition; an
/// <see cref="IConditionalTuple"/> needs an <see cref="IConditionalForm"/> of such a form
/// that names the same condition;</item>
/// <item>for an <see cref="IConditionalTuple"/>, the condition name is 2 to 256 code
/// points with no space or control character, the model defines the condition, and the
/// context is a JSON object whose keys are all parameters of that condition.</item>
/// </list>
/// Throws <see cref="ArgumentException"/> when the subject, or a rewrite or form the check
/// reaches, is none of the known variants.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/openfga</c> <c>internal/validation/validation.go</c>,
/// <c>ValidateTupleForWrite</c> (<c>ValidateUserObjectRelation</c>,
/// <c>validateTuplesetRestrictions</c>, <c>validateTypeRestrictions</c>,
/// <c>validateCondition</c>); <c>openfga/openfga</c> <c>pkg/tuple/tuple.go</c>,
/// <c>IsValidRelation</c>; <c>openfga/api</c> <c>openfga/v1/openfga.proto</c>,
/// <c>TupleKey.relation</c> pattern <c>^[^:#@\s]{1,50}$</c> and
/// <c>RelationshipCondition.name</c> pattern <c>^[^\s]{2,256}$</c>.
/// </remarks>
public sealed record AssignableTupleCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly ITuple _tuple;

    public AssignableTupleCondition(IAuthorizationModel model, ITuple tuple)
    {
        _model = model;
        _tuple = tuple;
    }

    public bool BoolValue =>
        new And(
            new KnownSubjectCondition(_model, _tuple.Subject),
            new ValidSubjectCondition(_tuple.Subject),
            new ValidObjectCondition(_tuple.Object),
            new IdentifierCondition(_tuple.Relation, new Int(50)),
            new Not(new TextContainingControlCharCondition(_tuple.Relation)),
            new RelationOfTypeCondition(_model, _tuple.Object.Type, _tuple.Relation),
            new TuplesetSubjectCondition(_model, _tuple),
            new AnyCondition<ISubjectForm>(
                new TypeRestrictions(
                    new RelationDefinitions(_model, _tuple.Object.Type, _tuple.Relation)
                ),
                form => new FormAdmitsTupleCondition(form, _tuple)
            ),
            new ConditionalTupleCondition(_model, _tuple)
        ).BoolValue;
}
