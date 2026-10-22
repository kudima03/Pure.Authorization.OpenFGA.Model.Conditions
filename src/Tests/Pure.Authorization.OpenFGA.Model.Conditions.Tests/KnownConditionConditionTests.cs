using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record KnownConditionConditionTests
{
    [Fact]
    public void TakesPositiveResultOnDefinedCondition()
    {
        IBool condition = new KnownConditionCondition(
            new FakeModel(
                [],
                [new FakeCondition("in_office"), new FakeCondition("non_expired")]
            ),
            new String("non_expired")
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnUndefinedCondition()
    {
        IBool condition = new KnownConditionCondition(
            new FakeModel(
                [],
                [new FakeCondition("in_office"), new FakeCondition("non_expired")]
            ),
            new String("expired")
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnModelWithoutConditions()
    {
        IBool condition = new KnownConditionCondition(
            new FakeModel(),
            new String("in_office")
        );

        Assert.False(condition.BoolValue);
    }
}
