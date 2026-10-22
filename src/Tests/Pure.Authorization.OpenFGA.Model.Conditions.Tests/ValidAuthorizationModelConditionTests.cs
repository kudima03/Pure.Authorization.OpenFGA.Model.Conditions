using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Fixtures;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidAuthorizationModelConditionTests
{
    private static readonly IAuthorizationModel Warden = new WardenModel();

    [Fact]
    public void TakesPositiveResultOnWardenModel()
    {
        IBool condition = new ValidAuthorizationModelCondition(Warden);

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnSchemaVersion12()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithSchemaVersion(Warden, "1.2")
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUnsupportedSchemaVersion()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithSchemaVersion(Warden, "1.0")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnModelWithoutTypes()
    {
        IBool condition = new ValidAuthorizationModelCondition(new FakeModel());

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnSingleTypeWithoutRelations()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new FakeModel(new FakeType("user"))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnDuplicateType()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new FakeModel(Warden.Types.Append(new FakeType("user")), [])
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnReservedTypeName()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithType(Warden, new FakeType("this"))
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedTypeInTypeRestriction()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithoutType(Warden, "client")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnDuplicateRelation()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithType(
                Warden,
                new FakeType(
                    "folder",
                    new ModelType(Warden, "folder").Relations.Append(
                        new FakeRelation(
                            "owner",
                            new FakeDirect(new FakeTypeForm("user"))
                        )
                    )
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidRelationName()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation("can#share", new FakeComputed("owner"))
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedComputedRelation()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation(
                    "can_grant",
                    new FakeUnion(new FakeComputed("owner"), new FakeComputed("manager"))
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnRelationComputedFromItself()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "workspace",
                new FakeRelation(
                    "viewer",
                    new FakeUnion(
                        new FakeDirect(new FakeTypeForm("user")),
                        new FakeComputed("viewer")
                    )
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedTupleset()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "widget",
                new FakeRelation("viewer", new FakeFrom("viewer", "space"))
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetDefinedByRewrite()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation(
                    "parent",
                    new FakeUnion(
                        new FakeDirect(new FakeTypeForm("folder")),
                        new FakeComputed("owner")
                    )
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetAssignableFromSubjectSet()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation(
                    "parent",
                    new FakeDirect(
                        new FakeTypeForm("folder"),
                        new FakeSubjectSetForm("folder", "owner")
                    )
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetAssignableFromWildcard()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "query",
                new FakeRelation(
                    "visualization",
                    new FakeDirect(
                        new FakeTypeForm("chart"),
                        new FakeTypeForm("diagram"),
                        new FakeWildcardForm("chart")
                    )
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnRelationUndefinedOnTuplesetTypes()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "query",
                new FakeRelation("viewer", new FakeFrom("reader", "visualization"))
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnAssignableRelationWithoutTypes()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "workspace",
                new FakeRelation("owner", new FakeDirect())
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedSubjectSetRelation()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "organization",
                new FakeRelation(
                    "admin",
                    new FakeDirect(
                        new FakeTypeForm("user"),
                        new FakeSubjectSetForm("group", "owner")
                    )
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnConditionalForm()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithConditions(
                new ModelWithRelation(
                    Warden,
                    "folder",
                    new FakeRelation(
                        "owner",
                        new FakeDirect(
                            new FakeConditionalForm(
                                new FakeTypeForm("user"),
                                "non_expired"
                            )
                        )
                    )
                ),
                NonExpired()
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionalFormOfUndefinedCondition()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation(
                    "owner",
                    new FakeDirect(
                        new FakeConditionalForm(new FakeTypeForm("user"), "non_expired")
                    )
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnMaximumConditions()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithConditions(Warden, Conditions(25))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnMoreThanMaximumConditions()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithConditions(Warden, Conditions(26))
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnDuplicateCondition()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithConditions(Warden, NonExpired(), NonExpired())
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidConditionName()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithConditions(Warden, new FakeCondition("non expired"))
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnExpressionOfMaximumBytes()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithConditions(
                Warden,
                new FakeCondition("long", new string('é', 256), [])
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnExpressionLongerThanMaximumBytes()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithConditions(
                Warden,
                new FakeCondition("long", new string('é', 256) + "1", [])
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidConditionParameter()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithConditions(
                Warden,
                new FakeCondition("non_expired", new FakeParameter("grant_time", "date"))
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownRewrite()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation("owner", new UnknownRewrite())
            )
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownSubjectForm()
    {
        IBool condition = new ValidAuthorizationModelCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation("owner", new FakeDirect(new UnknownSubjectForm()))
            )
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    private static ICondition NonExpired()
    {
        return new FakeCondition(
            "non_expired",
            "current_time < grant_time + duration",
            [
                new FakeParameter("grant_time", "timestamp"),
                new FakeParameter("duration", "duration"),
            ]
        );
    }

    private static IEnumerable<ICondition> Conditions(int count)
    {
        return Enumerable.Range(0, count).Select(index => new FakeCondition($"c{index}"));
    }
}
