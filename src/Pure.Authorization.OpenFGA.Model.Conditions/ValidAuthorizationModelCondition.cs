using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.Bool;
using Pure.Primitives.Bool.Operations;
using Pure.Primitives.Number;
using Pure.Primitives.Number.Operations;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Holds when an authorization model passes every rule this package checks:
/// <list type="bullet">
/// <item>its schema version is a <see cref="SupportedSchemaVersionCondition"/>;</item>
/// <item>it has at least one type, and type names are unique
/// (<see cref="UniqueTypeNamesCondition"/>);</item>
/// <item>every type has a <see cref="ValidTypeNameCondition"/> and unique relation names
/// (<see cref="UniqueRelationNamesCondition"/>), and every relation has a
/// <see cref="ValidRelationNameCondition"/>, a <see cref="ResolvableRewriteCondition"/>
/// and <see cref="ValidTypeRestrictionsCondition"/>;</item>
/// <item>it has at most 25 conditions with unique names
/// (<see cref="UniqueConditionNamesCondition"/>), and every condition has a
/// <see cref="ValidConditionNameCondition"/>, an expression of at most 512 bytes in UTF-8
/// and <see cref="ValidConditionParametersCondition"/>.</item>
/// </list>
/// It does not check every rule OpenFGA enforces; the README lists the rules it leaves
/// out. Throws <see cref="ArgumentException"/> when the model holds an
/// <see cref="Abstractions.Rewrite.IRewrite"/> or
/// <see cref="Abstractions.SubjectForm.ISubjectForm"/> that is none of the known
/// variants.
/// </summary>
/// <remarks>
/// Sources: <c>openfga/openfga</c> <c>pkg/typesystem/typesystem.go</c>,
/// <c>NewAndValidate</c>; <c>openfga/api</c> <c>openfga/v1/openfga_service.proto</c>,
/// <c>WriteAuthorizationModelRequest.type_definitions</c> (<c>min_items: 1</c>) and
/// <c>conditions</c> (<c>max_pairs: 25</c>); <c>openfga/api</c>
/// <c>openfga/v1/authzmodel.proto</c>, <c>Condition.expression</c>
/// (<c>max_bytes: 512</c>).
/// </remarks>
public sealed record ValidAuthorizationModelCondition : IBool
{
    private readonly IAuthorizationModel _model;

    public ValidAuthorizationModelCondition(IAuthorizationModel model)
    {
        _model = model;
    }

    public bool BoolValue =>
        new And(
            new SupportedSchemaVersionCondition(_model.SchemaVersion),
            new NotEmptyCondition<ITypeDefinition>(_model.Types),
            new UniqueTypeNamesCondition(_model),
            new AllCondition<ITypeDefinition>(
                _model.Types,
                type => new And(
                    new ValidTypeNameCondition(type.Name),
                    new UniqueRelationNamesCondition(type),
                    new AllCondition<IRelationDefinition>(
                        type.Relations,
                        relation => new And(
                            new ValidRelationNameCondition(relation.Name),
                            new ResolvableRewriteCondition(_model, type, relation),
                            new ValidTypeRestrictionsCondition(_model, relation)
                        )
                    )
                )
            ),
            new LessThanOrEqualCondition<int>(
                new ItemCount<ICondition>(_model.Conditions),
                new Int(25)
            ),
            new UniqueConditionNamesCondition(_model),
            new AllCondition<ICondition>(
                _model.Conditions,
                condition => new And(
                    new ValidConditionNameCondition(condition.Name),
                    new LessThanOrEqualCondition<int>(
                        new Utf8ByteLength(condition.Expression),
                        new Int(512)
                    ),
                    new ValidConditionParametersCondition(condition)
                )
            )
        ).BoolValue;
}
