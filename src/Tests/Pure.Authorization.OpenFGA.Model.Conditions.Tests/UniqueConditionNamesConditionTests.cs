using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record UniqueConditionNamesConditionTests
{
    [Fact]
    public void TakesPositiveResultOnUniqueNames()
    {
        IBool condition = new UniqueConditionNamesCondition(
            new FakeModel(
                [],
                [new FakeCondition("in_office"), new FakeCondition("non_expired")]
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnDuplicateName()
    {
        IBool condition = new UniqueConditionNamesCondition(
            new FakeModel(
                [],
                [new FakeCondition("in_office"), new FakeCondition("in_office")]
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnModelWithoutConditions()
    {
        IBool condition = new UniqueConditionNamesCondition(new FakeModel());

        Assert.True(condition.BoolValue);
    }
}
