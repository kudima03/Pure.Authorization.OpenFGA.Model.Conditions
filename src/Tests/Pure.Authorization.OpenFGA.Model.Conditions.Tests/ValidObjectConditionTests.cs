using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidObjectConditionTests
{
    [Theory]
    [InlineData("document", "1")]
    [InlineData("a", "b")]
    [InlineData("user", "anne@example.com")]
    [InlineData("a@b", "1")]
    [InlineData("a*b", "1")]
    [InlineData("doc", "a*")]
    [InlineData("doc", "2021-budget|v1")]
    [InlineData("doc", "a b")]
    public void TakesPositiveResultOnValidObject(string type, string id)
    {
        IBool condition = new ValidObjectCondition(new FakeObject(type, id));

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("", "1")]
    [InlineData("doc", "")]
    [InlineData("a:b", "1")]
    [InlineData("a#b", "1")]
    [InlineData("a b", "1")]
    [InlineData("a\tb", "1")]
    [InlineData("a\u0001b", "1")]
    [InlineData("doc", "a:b")]
    [InlineData("doc", "a#b")]
    [InlineData("doc", "a b")]
    [InlineData("doc", "a\nb")]
    [InlineData("doc", "a\u007fb")]
    [InlineData("doc", "a\u0085b")]
    [InlineData("doc", "*")]
    public void TakesNegativeResultOnInvalidObject(string type, string id)
    {
        IBool condition = new ValidObjectCondition(new FakeObject(type, id));

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnObjectOfMaximumLength()
    {
        IBool condition = new ValidObjectCondition(
            new FakeObject("t", new string('a', 254))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnObjectLongerThanMaximum()
    {
        IBool condition = new ValidObjectCondition(
            new FakeObject("t", new string('a', 255))
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void CountsLengthInCodePoints()
    {
        IBool condition = new ValidObjectCondition(
            new FakeObject("t", string.Concat(Enumerable.Repeat("\U0001F600", 254)))
        );

        Assert.True(condition.BoolValue);
    }
}
