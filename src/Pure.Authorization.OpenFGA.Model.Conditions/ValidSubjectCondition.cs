using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Authorization.OpenFGA.Model.Abstractions.Subject;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number;
using Pure.Primitives.Number.Operations;
using Pure.Primitives.String;
using Pure.Primitives.String.Operations;
using String = Pure.Primitives.String.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a subject is valid as the subject (OpenFGA <c>user</c>) of a relationship
/// tuple, at most 512 bytes long in UTF-8 as written, and:
/// <list type="bullet">
/// <item>an <see cref="IObject"/> (<c>type:id</c>) has a type and an id that are not
/// empty and contain no <c>:</c>, <c>#</c>, space or control character, and an id that
/// is not the wildcard <c>*</c>, which would make it a typed wildcard;</item>
/// <item>an <see cref="ISubjectSet"/> (<c>type:id#relation</c>) has a type that is not
/// empty and contains no <c>:</c>, <c>#</c>, space or control character, and an id and a
/// relation that are not empty and contain no <c>:</c>, <c>#</c>, <c>*</c>, space or
/// control character;</item>
/// <item>an <see cref="ITypedWildcard"/> (<c>type:*</c>) has a type that is not empty
/// and contains no <c>:</c>, <c>#</c>, space or control character.</item>
/// </list>
/// Throws <see cref="ArgumentException"/> for an <see cref="ISubject"/> that is none of
/// the known variants.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/api</c> <c>openfga/v1/openfga.proto</c>, <c>TupleKey.user</c>
/// <c>max_bytes: 512</c>; <c>openfga/openfga</c> <c>pkg/tuple/tuple.go</c>,
/// <c>IsValidObject</c>, <c>IsValidUserset</c> and <c>IsTypedWildcard</c>;
/// <c>openfga/openfga</c> <c>internal/validation/validation.go</c>, <c>ValidateUser</c>.
/// </remarks>
public sealed record ValidSubjectCondition : IBool
{
    private readonly ISubject _subject;

    public ValidSubjectCondition(ISubject subject)
    {
        _subject = subject;
    }

    public bool BoolValue =>
        _subject switch
        {
            IObject @object => new And(
                new TupleFieldPartCondition(@object.Type, new String(":# ")),
                new TupleFieldPartCondition(@object.Id, new String(":# ")),
                new Not(new StringEqualCondition(@object.Id, new String("*"))),
                new LessThanOrEqualCondition<int>(
                    new Utf8ByteLength(new ColonJoinedString(@object.Type, @object.Id)),
                    new Int(512)
                )
            ).BoolValue,
            ISubjectSet subjectSet => new And(
                new TupleFieldPartCondition(subjectSet.Object.Type, new String(":# ")),
                new TupleFieldPartCondition(subjectSet.Object.Id, new String(":#* ")),
                new TupleFieldPartCondition(subjectSet.Relation, new String(":#* ")),
                new LessThanOrEqualCondition<int>(
                    new Utf8ByteLength(
                        new JoinedString(
                            new HashString(),
                            [
                                new ColonJoinedString(
                                    subjectSet.Object.Type,
                                    subjectSet.Object.Id
                                ),
                                subjectSet.Relation,
                            ]
                        )
                    ),
                    new Int(512)
                )
            ).BoolValue,
            ITypedWildcard wildcard => new And(
                new TupleFieldPartCondition(wildcard.Type, new String(":# ")),
                new LessThanOrEqualCondition<int>(
                    new Utf8ByteLength(
                        new ColonJoinedString(wildcard.Type, new AsteriskString())
                    ),
                    new Int(512)
                )
            ).BoolValue,
            _ => throw new ArgumentException(),
        };
}
