using System.Text.Json.Nodes;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Fixtures;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record AssignableTupleConditionTests
{
    private static readonly IAuthorizationModel Warden = new WardenModel();

    [Theory]
    [InlineData("user", "anne", "viewer", "folder")]
    [InlineData("user", "anne", "owner", "chart")]
    [InlineData("folder", "2", "parent", "folder")]
    [InlineData("organization", "acme", "organization", "datasource")]
    [InlineData("chart", "1", "visualization", "query")]
    [InlineData("client", "etl", "ingester", "datasource")]
    public void TakesPositiveResultOnObjectSubjectOfWardenModel(
        string subjectType,
        string subjectId,
        string relation,
        string objectType
    )
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeObject(subjectType, subjectId),
                relation,
                new FakeObject(objectType, "1")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("group", "member", "viewer", "folder")]
    [InlineData("organization", "member", "viewer", "folder")]
    [InlineData("group", "member", "blocked", "column")]
    public void TakesPositiveResultOnSubjectSetOfWardenModel(
        string subjectType,
        string subjectRelation,
        string relation,
        string objectType
    )
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeSubjectSet(subjectType, "eng", subjectRelation),
                relation,
                new FakeObject(objectType, "1")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("client", "etl", "viewer", "folder")]
    [InlineData("group", "eng", "viewer", "folder")]
    [InlineData("user", "anne", "ingester", "datasource")]
    [InlineData("user", "anne", "can_grant", "folder")]
    [InlineData("user", "anne", "editor", "widget")]
    [InlineData("user", "anne", "parent", "folder")]
    [InlineData("diagram", "1", "folder", "chart")]
    public void TakesNegativeResultOnObjectSubjectNotAdmitted(
        string subjectType,
        string subjectId,
        string relation,
        string objectType
    )
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeObject(subjectType, subjectId),
                relation,
                new FakeObject(objectType, "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnSubjectSetOfOtherRelation()
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeSubjectSet("group", "eng", "admin"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnSubjectSetOfOtherType()
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeSubjectSet("organization", "acme", "member"),
                "owner",
                new FakeObject("chart", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnWildcardWithoutWildcardForm()
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeWildcard("user"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedObjectType()
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeObject("user", "anne"),
                "viewer",
                new FakeObject("report", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedRelation()
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeObject("user", "anne"),
                "reader",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidObject()
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeObject("user", "anne"),
                "viewer",
                new FakeObject("folder", "*")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidSubject()
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(
                new FakeObject("user", "anne smith"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Theory]
    [InlineData("a b")]
    [InlineData("a@b")]
    [InlineData("a\u0001b")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public void TakesNegativeResultOnInvalidRelationDefinedByModel(string relation)
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation(relation, new FakeDirect(new FakeTypeForm("user")))
            ),
            new FakeTuple(
                new FakeObject("user", "anne"),
                relation,
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnRelationOfMaximumLength()
    {
        string relation = new('a', 50);

        IBool condition = new AssignableTupleCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation(relation, new FakeDirect(new FakeTypeForm("user")))
            ),
            new FakeTuple(
                new FakeObject("user", "anne"),
                relation,
                new FakeObject("folder", "1")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnSubjectOfUndefinedType()
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation("owner", new FakeDirect(new FakeTypeForm("robot")))
            ),
            new FakeTuple(
                new FakeObject("robot", "r2"),
                "owner",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnWildcardOfUndefinedType()
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation("owner", new FakeDirect(new FakeWildcardForm("robot")))
            ),
            new FakeTuple(
                new FakeWildcard("robot"),
                "owner",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnSubjectSetOfUndefinedRelation()
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation(
                    "owner",
                    new FakeDirect(new FakeSubjectSetForm("group", "owner"))
                )
            ),
            new FakeTuple(
                new FakeSubjectSet("group", "eng", "owner"),
                "owner",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnWildcardWithWildcardForm()
    {
        IBool condition = new AssignableTupleCondition(
            WithViewer(
                new FakeDirect(new FakeTypeForm("user"), new FakeWildcardForm("user"))
            ),
            new FakeTuple(
                new FakeWildcard("user"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnWildcardOfOtherType()
    {
        IBool condition = new AssignableTupleCondition(
            WithViewer(new FakeDirect(new FakeWildcardForm("user"))),
            new FakeTuple(
                new FakeWildcard("client"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnObjectWithWildcardFormOnly()
    {
        IBool condition = new AssignableTupleCondition(
            WithViewer(new FakeDirect(new FakeWildcardForm("user"))),
            new FakeTuple(
                new FakeObject("user", "anne"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnFormInDifference()
    {
        IBool condition = new AssignableTupleCondition(
            WithViewer(
                new FakeDifference(
                    new FakeComputed("owner"),
                    new FakeDirect(new FakeTypeForm("client"))
                )
            ),
            new FakeTuple(
                new FakeObject("client", "etl"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnSubjectSetForTupleset()
    {
        IBool condition = new AssignableTupleCondition(
            WithParent(
                new FakeDirect(
                    new FakeTypeForm("folder"),
                    new FakeSubjectSetForm("folder", "owner")
                )
            ),
            new FakeTuple(
                new FakeSubjectSet("folder", "2", "owner"),
                "parent",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnWildcardForTupleset()
    {
        IBool condition = new AssignableTupleCondition(
            WithParent(
                new FakeDirect(new FakeTypeForm("folder"), new FakeWildcardForm("folder"))
            ),
            new FakeTuple(
                new FakeWildcard("folder"),
                "parent",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetDefinedByRewrite()
    {
        IBool condition = new AssignableTupleCondition(
            WithParent(
                new FakeUnion(
                    new FakeDirect(new FakeTypeForm("folder")),
                    new FakeComputed("owner")
                )
            ),
            new FakeTuple(
                new FakeObject("folder", "2"),
                "parent",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetDuplicatedByRewrite()
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithType(
                Warden,
                new FakeType(
                    "folder",
                    new ModelType(Warden, "folder").Relations.Append(
                        new FakeRelation(
                            "parent",
                            new FakeUnion(
                                new FakeDirect(new FakeTypeForm("folder")),
                                new FakeComputed("owner")
                            )
                        )
                    )
                )
            ),
            new FakeTuple(
                new FakeObject("folder", "2"),
                "parent",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnSubjectSetForRelationNamedLikeOtherTypesTupleset()
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithRelation(
                Warden,
                "workspace",
                new FakeRelation(
                    "parent",
                    new FakeDirect(new FakeSubjectSetForm("group", "member"))
                )
            ),
            new FakeTuple(
                new FakeSubjectSet("group", "eng", "member"),
                "parent",
                new FakeObject("workspace", "1")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnConditionalTupleOfConditionalForm()
    {
        IBool condition = new AssignableTupleCondition(
            Conditional(),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeObject("user", "anne"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                "non_expired"
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnPlainTupleOfConditionalFormOnly()
    {
        IBool condition = new AssignableTupleCondition(
            Conditional(),
            new FakeTuple(
                new FakeObject("user", "anne"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnPlainTupleOfPlainForm()
    {
        IBool condition = new AssignableTupleCondition(
            Conditional(),
            new FakeTuple(
                new FakeSubjectSet("group", "eng", "member"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionalTupleOfPlainFormOnly()
    {
        IBool condition = new AssignableTupleCondition(
            Conditional(),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeSubjectSet("group", "eng", "member"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                "non_expired"
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionalTupleOfOtherCondition()
    {
        IBool condition = new AssignableTupleCondition(
            Conditional(),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeObject("user", "anne"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                "in_office"
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnConditionalWildcard()
    {
        IBool condition = new AssignableTupleCondition(
            Conditional(),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeWildcard("user"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                "in_office"
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionalObjectOfConditionalWildcardForm()
    {
        IBool condition = new AssignableTupleCondition(
            Conditional(),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeObject("client", "etl"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                "in_office"
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionalFormOfConditionalForm()
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithConditions(
                WithViewer(
                    new FakeDirect(
                        new FakeConditionalForm(
                            new FakeConditionalForm(
                                new FakeTypeForm("user"),
                                "non_expired"
                            ),
                            "non_expired"
                        )
                    )
                ),
                new FakeCondition("non_expired")
            ),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeObject("user", "anne"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                "non_expired"
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedCondition()
    {
        IBool condition = new AssignableTupleCondition(
            WithViewer(
                new FakeDirect(new FakeConditionalForm(new FakeTypeForm("user"), "ghost"))
            ),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeObject("user", "anne"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                "ghost"
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a b")]
    [InlineData("a:b@c")]
    public void TakesPositiveResultOnConditionNameAcceptedByOpenFga(string name)
    {
        Assert.True(ConditionNamed(name).BoolValue);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("a b")]
    [InlineData("a\u0001b")]
    [InlineData("a\tb")]
    public void TakesNegativeResultOnConditionNameRejectedByOpenFga(string name)
    {
        Assert.False(ConditionNamed(name).BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnConditionNameOfMaximumLength()
    {
        Assert.True(ConditionNamed(new string('c', 256)).BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionNameLongerThanMaximum()
    {
        Assert.False(ConditionNamed(new string('c', 257)).BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnContextOfConditionParameters()
    {
        Assert.True(
            WithContext(
                new JsonObject
                {
                    ["grant_time"] = "2026-01-01T00:00:00Z",
                    ["duration"] = "1h",
                }
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnContextOfUnknownParameter()
    {
        Assert.False(
            WithContext(
                new JsonObject
                {
                    ["grant_time"] = "2026-01-01T00:00:00Z",
                    ["ip"] = "127.0.0.1",
                }
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnContextOfOtherConditionParameter()
    {
        Assert.False(WithContext(new JsonObject { ["office"] = "berlin" }).BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnContextThatIsNotObject()
    {
        Assert.False(WithContext(new JsonArray()).BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownSubject()
    {
        IBool condition = new AssignableTupleCondition(
            Warden,
            new FakeTuple(new UnknownSubject(), "viewer", new FakeObject("folder", "1"))
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownForm()
    {
        IBool condition = new AssignableTupleCondition(
            WithViewer(new FakeDirect(new UnknownSubjectForm())),
            new FakeTuple(
                new FakeObject("user", "anne"),
                "viewer",
                new FakeObject("folder", "1")
            )
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownFormForConditionalTuple()
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithConditions(
                WithViewer(new FakeDirect(new UnknownSubjectForm())),
                new FakeCondition("non_expired")
            ),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeObject("user", "anne"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                "non_expired"
            )
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownRewriteOfObjectType()
    {
        IBool condition = new AssignableTupleCondition(
            new ModelWithRelation(
                Warden,
                "folder",
                new FakeRelation("audit", new UnknownRewrite())
            ),
            new FakeTuple(
                new FakeObject("user", "anne"),
                "owner",
                new FakeObject("folder", "1")
            )
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    private static IAuthorizationModel WithViewer(IRewrite rewrite)
    {
        return new ModelWithRelation(
            Warden,
            "folder",
            new FakeRelation("viewer", rewrite)
        );
    }

    private static IAuthorizationModel WithParent(IRewrite rewrite)
    {
        return new ModelWithRelation(
            Warden,
            "folder",
            new FakeRelation("parent", rewrite)
        );
    }

    // define viewer: [user with non_expired, user:* with in_office, group#member]
    private static IAuthorizationModel Conditional()
    {
        return new ModelWithConditions(
            WithViewer(
                new FakeDirect(
                    new FakeConditionalForm(new FakeTypeForm("user"), "non_expired"),
                    new FakeConditionalForm(new FakeWildcardForm("user"), "in_office"),
                    new FakeConditionalForm(new FakeTypeForm("client"), "non_expired"),
                    new FakeSubjectSetForm("group", "member")
                )
            ),
            new FakeCondition(
                "non_expired",
                "current_time < grant_time + duration",
                [
                    new FakeParameter("grant_time", "timestamp"),
                    new FakeParameter("duration", "duration"),
                ]
            ),
            new FakeCondition(
                "in_office",
                "office == 'berlin'",
                [new FakeParameter("office", "string")]
            )
        );
    }

    private static AssignableTupleCondition ConditionNamed(string name)
    {
        return new AssignableTupleCondition(
            new ModelWithConditions(
                WithViewer(
                    new FakeDirect(
                        new FakeConditionalForm(new FakeTypeForm("user"), name)
                    )
                ),
                new FakeCondition(name)
            ),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeObject("user", "anne"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                name
            )
        );
    }

    private static AssignableTupleCondition WithContext(JsonNode context)
    {
        return new AssignableTupleCondition(
            Conditional(),
            new FakeConditionalTuple(
                new FakeTuple(
                    new FakeObject("user", "anne"),
                    "viewer",
                    new FakeObject("folder", "1")
                ),
                new FakeRelationshipCondition("non_expired", context)
            )
        );
    }
}
