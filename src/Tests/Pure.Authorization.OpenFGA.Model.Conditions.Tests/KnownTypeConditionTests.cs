using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record KnownTypeConditionTests
{
    [Fact]
    public void TakesPositiveResultOnDefinedType()
    {
        IBool condition = new KnownTypeCondition(
            new FakeModel(new FakeType("user"), new FakeType("document")),
            new String("document")
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedType()
    {
        IBool condition = new KnownTypeCondition(
            new FakeModel(new FakeType("user"), new FakeType("document")),
            new String("folder")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnTypeDifferingInCase()
    {
        IBool condition = new KnownTypeCondition(
            new FakeModel(new FakeType("user")),
            new String("User")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnModelWithoutTypes()
    {
        IBool condition = new KnownTypeCondition(new FakeModel(), new String("user"));

        Assert.False(condition.BoolValue);
    }
}
