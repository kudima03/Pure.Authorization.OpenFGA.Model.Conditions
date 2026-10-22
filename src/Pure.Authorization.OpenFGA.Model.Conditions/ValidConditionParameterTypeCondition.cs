using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number;
using Pure.Primitives.Number.Operations;
using String = Pure.Primitives.String.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a condition parameter type is one OpenFGA can decode: <c>any</c>,
/// <c>bool</c>, <c>string</c>, <c>int</c>, <c>uint</c>, <c>double</c>, <c>duration</c>,
/// <c>timestamp</c> or <c>ipaddress</c> with no generic types, or <c>map</c> or
/// <c>list</c> with exactly one generic type that is itself valid.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/api</c> <c>openfga/v1/authzmodel.proto</c>,
/// <c>ConditionParamTypeRef.TypeName</c>; <c>openfga/openfga</c>
/// <c>internal/condition/types/types.go</c> (the type names) and
/// <c>internal/condition/types/encoding.go</c>, <c>DecodeParameterType</c> (the generic
/// type count of each type, checked recursively).
/// </remarks>
public sealed record ValidConditionParameterTypeCondition : IBool
{
    private readonly IConditionParameterType _type;

    public ValidConditionParameterTypeCondition(IConditionParameterType type)
    {
        _type = type;
    }

    public bool BoolValue =>
        new Or(
            new And(
                new AnyCondition<IString>(
                    [
                        new String("any"),
                        new String("bool"),
                        new String("string"),
                        new String("int"),
                        new String("uint"),
                        new String("double"),
                        new String("duration"),
                        new String("timestamp"),
                        new String("ipaddress"),
                    ],
                    name => new StringEqualCondition(_type.Name, name)
                ),
                new Not(
                    new NotEmptyCondition<IConditionParameterType>(_type.GenericTypes)
                )
            ),
            new And(
                new AnyCondition<IString>(
                    [new String("map"), new String("list")],
                    name => new StringEqualCondition(_type.Name, name)
                ),
                new EqualCondition<int>(
                    new ItemCount<IConditionParameterType>(_type.GenericTypes),
                    new Int(1)
                ),
                new AllCondition<IConditionParameterType>(
                    _type.GenericTypes,
                    generic => new ValidConditionParameterTypeCondition(generic)
                )
            )
        ).BoolValue;
}
