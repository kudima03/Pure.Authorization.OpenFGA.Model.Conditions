using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidTypeRestrictionsConditionTests
{
    private static readonly IAuthorizationModel Model = new FakeModel(
        [
            new FakeType("user"),
            new FakeType(
                "group",
                new FakeRelation("member", new FakeDirect(new FakeTypeForm("user")))
            ),
        ],
        [new FakeCondition("non_expired")]
    );

    [Fact]
    public void TakesPositiveResultOnRelationWithoutDirect()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation("viewer", new FakeComputed("member"))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnDirectWithValidForms()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation(
                "viewer",
                new FakeDirect(
                    new FakeTypeForm("user"),
                    new FakeWildcardForm("user"),
                    new FakeSubjectSetForm("group", "member"),
                    new FakeConditionalForm(new FakeTypeForm("user"), "non_expired")
                )
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnDirectWithoutForms()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation("viewer", new FakeDirect())
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnNestedDirectWithoutForms()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation(
                "viewer",
                new FakeUnion(new FakeComputed("owner"), new FakeDirect())
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnFormsSpreadOverDirects()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation(
                "viewer",
                new FakeUnion(new FakeDirect(), new FakeDirect(new FakeTypeForm("user")))
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidForm()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation(
                "viewer",
                new FakeDirect(new FakeTypeForm("user"), new FakeTypeForm("client"))
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidFormInDifferenceBase()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation(
                "viewer",
                new FakeDifference(
                    new FakeDirect(new FakeTypeForm("client")),
                    new FakeComputed("blocked")
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidFormInDifferenceSubtract()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation(
                "viewer",
                new FakeDifference(
                    new FakeComputed("member"),
                    new FakeDirect(new FakeSubjectSetForm("group", "owner"))
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidFormInIntersection()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation(
                "viewer",
                new FakeIntersection(
                    new FakeComputed("member"),
                    new FakeDirect(new FakeWildcardForm("client"))
                )
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownRewrite()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation(
                "viewer",
                new FakeUnion(new FakeComputed("member"), new UnknownRewrite())
            )
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownForm()
    {
        IBool condition = new ValidTypeRestrictionsCondition(
            Model,
            new FakeRelation("viewer", new FakeDirect(new UnknownSubjectForm()))
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }
}
