using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Abstractions.String;
using Pure.Primitives.Bool.Operations;
using String = Pure.Primitives.String.String;
using StringEqualCondition = Pure.Primitives.String.Operations.EqualCondition;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when a schema version is one OpenFGA accepts for an authorization model:
/// <c>1.1</c> or <c>1.2</c>. Schema <c>1.0</c> is rejected.
/// </summary>
/// <remarks>
/// Source: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>IsSchemaVersionSupported</c>, checked first by <c>NewAndValidate</c>.
/// </remarks>
public sealed record SupportedSchemaVersionCondition : IBool
{
    private readonly IString _schemaVersion;

    public SupportedSchemaVersionCondition(IString schemaVersion)
    {
        _schemaVersion = schemaVersion;
    }

    public bool BoolValue =>
        new Or(
            new StringEqualCondition(_schemaVersion, new String("1.1")),
            new StringEqualCondition(_schemaVersion, new String("1.2"))
        ).BoolValue;
}
