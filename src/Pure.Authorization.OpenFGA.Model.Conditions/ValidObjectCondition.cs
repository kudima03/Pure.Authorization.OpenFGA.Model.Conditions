using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number;
using Pure.Primitives.Number.Operations;
using Pure.Primitives.String.Operations;
using String = Pure.Primitives.String.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when an object is valid as the object of a relationship tuple, written as
/// <c>type:id</c>: the type and the id are not empty and contain no <c>:</c>, <c>#</c>,
/// space or control character; the id is not the wildcard <c>*</c>; and <c>type:id</c> is
/// at most 256 code points long.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/api</c> <c>openfga/v1/openfga.proto</c>, <c>TupleKey.object</c>
/// pattern <c>^[^\s]{2,256}$</c>; <c>openfga/openfga</c> <c>pkg/tuple/tuple.go</c>,
/// <c>IsValidObject</c>; <c>openfga/openfga</c> <c>internal/validation/validation.go</c>,
/// <c>ValidateObject</c> (rejects a typed wildcard as the object).
/// </remarks>
public sealed record ValidObjectCondition : IBool
{
    private readonly IObject _object;

    public ValidObjectCondition(IObject @object)
    {
        _object = @object;
    }

    public bool BoolValue =>
        new And(
            new TupleFieldPartCondition(_object.Type, new String(":# ")),
            new TupleFieldPartCondition(_object.Id, new String(":# ")),
            new Not(new StringEqualCondition(_object.Id, new String("*"))),
            new LessThanOrEqualCondition<int>(
                new RuneLength(new ColonJoinedString(_object.Type, _object.Id)),
                new Int(256)
            )
        ).BoolValue;
}
