using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record UniqueTypeNamesConditionTests
{
    [Fact]
    public void TakesPositiveResultOnUniqueNames()
    {
        IBool condition = new UniqueTypeNamesCondition(
            new FakeModel(
                new FakeType("user"),
                new FakeType("User"),
                new FakeType("document")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnDuplicateName()
    {
        IBool condition = new UniqueTypeNamesCondition(
            new FakeModel(
                new FakeType("user"),
                new FakeType("document"),
                new FakeType("user")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnModelWithoutTypes()
    {
        IBool condition = new UniqueTypeNamesCondition(new FakeModel());

        Assert.True(condition.BoolValue);
    }
}
