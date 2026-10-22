using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Authorization.OpenFGA.Model.Abstractions.Subject;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when the model defines the type of a subject and, for an
/// <see cref="ISubjectSet"/>, its relation on that type. Throws
/// <see cref="ArgumentException"/> for an <see cref="ISubject"/> that is none of the known
/// variants.
/// </summary>
internal sealed record KnownSubjectCondition : IBool
{
    private readonly IAuthorizationModel _model;

    private readonly ISubject _subject;

    public KnownSubjectCondition(IAuthorizationModel model, ISubject subject)
    {
        _model = model;
        _subject = subject;
    }

    public bool BoolValue =>
        _subject switch
        {
            IObject @object => new KnownTypeCondition(_model, @object.Type).BoolValue,
            ISubjectSet subjectSet => new RelationOfTypeCondition(
                _model,
                subjectSet.Object.Type,
                subjectSet.Relation
            ).BoolValue,
            ITypedWildcard wildcard => new KnownTypeCondition(
                _model,
                wildcard.Type
            ).BoolValue,
            _ => throw new ArgumentException(),
        };
}
