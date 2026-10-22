using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidSubjectFormConditionTests
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
    public void TakesPositiveResultOnTypeFormOfDefinedType()
    {
        IBool condition = new ValidSubjectFormCondition(Model, new FakeTypeForm("user"));

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnTypeFormOfUndefinedType()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeTypeForm("client")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnSubjectSetFormOfDefinedRelation()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeSubjectSetForm("group", "member")
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnSubjectSetFormOfUndefinedRelation()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeSubjectSetForm("group", "admin")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnSubjectSetFormOfUndefinedType()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeSubjectSetForm("team", "member")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnWildcardFormOfDefinedType()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeWildcardForm("user")
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnWildcardFormOfUndefinedType()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeWildcardForm("client")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnConditionalFormOfDefinedCondition()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeConditionalForm(
                new FakeSubjectSetForm("group", "member"),
                "non_expired"
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionalFormOfUndefinedCondition()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeConditionalForm(new FakeTypeForm("user"), "in_office")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionalFormOfInvalidForm()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeConditionalForm(new FakeWildcardForm("client"), "non_expired")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnConditionalFormOfConditionalForm()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeConditionalForm(
                new FakeConditionalForm(new FakeTypeForm("user"), "non_expired"),
                "non_expired"
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownForm()
    {
        IBool condition = new ValidSubjectFormCondition(Model, new UnknownSubjectForm());

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnConditionalFormOfUnknownForm()
    {
        IBool condition = new ValidSubjectFormCondition(
            Model,
            new FakeConditionalForm(new UnknownSubjectForm(), "non_expired")
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }
}
