using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidConditionParametersConditionTests
{
    [Fact]
    public void TakesPositiveResultOnValidParameters()
    {
        IBool condition = new ValidConditionParametersCondition(
            new FakeCondition(
                "non_expired",
                new FakeParameter("grant_time", "timestamp"),
                new FakeParameter("duration", "duration"),
                new FakeParameter(
                    "tags",
                    new FakeParameterType("list", new FakeParameterType("string"))
                )
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnNoParameters()
    {
        IBool condition = new ValidConditionParametersCondition(
            new FakeCondition("always")
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnMaximumParameters()
    {
        IBool condition = new ValidConditionParametersCondition(
            new FakeCondition(
                "many",
                Enumerable
                    .Range(0, 25)
                    .Select(index => new FakeParameter($"p{index}", "int"))
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnMoreThanMaximumParameters()
    {
        IBool condition = new ValidConditionParametersCondition(
            new FakeCondition(
                "many",
                Enumerable
                    .Range(0, 26)
                    .Select(index => new FakeParameter($"p{index}", "int"))
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnDuplicateParameterName()
    {
        IBool condition = new ValidConditionParametersCondition(
            new FakeCondition(
                "non_expired",
                new FakeParameter("grant_time", "timestamp"),
                new FakeParameter("grant_time", "duration")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a:b")]
    [InlineData("a#b")]
    [InlineData("a@b")]
    [InlineData("a b")]
    [InlineData("a\nb")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public void TakesNegativeResultOnInvalidParameterName(string name)
    {
        IBool condition = new ValidConditionParametersCondition(
            new FakeCondition(
                "non_expired",
                new FakeParameter("grant_time", "timestamp"),
                new FakeParameter(name, "duration")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnParameterNameOfMaximumLength()
    {
        IBool condition = new ValidConditionParametersCondition(
            new FakeCondition(
                "non_expired",
                new FakeParameter(new string('a', 50), "duration")
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidParameterType()
    {
        IBool condition = new ValidConditionParametersCondition(
            new FakeCondition(
                "non_expired",
                new FakeParameter("grant_time", "timestamp"),
                new FakeParameter("tags", "list")
            )
        );

        Assert.False(condition.BoolValue);
    }
}
