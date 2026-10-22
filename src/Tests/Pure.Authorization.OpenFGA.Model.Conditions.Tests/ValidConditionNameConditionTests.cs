using Pure.Primitives.Abstractions.Bool;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidConditionNameConditionTests
{
    [Theory]
    [InlineData("non_expired")]
    [InlineData("a")]
    [InlineData("self")]
    [InlineData("this")]
    [InlineData("$expression")]
    [InlineData("a\vb")]
    public void TakesPositiveResultOnValidName(string name)
    {
        IBool condition = new ValidConditionNameCondition(new String(name));

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a:b")]
    [InlineData("a#b")]
    [InlineData("a@b")]
    [InlineData("a b")]
    [InlineData("a\tb")]
    [InlineData("a\nb")]
    [InlineData("a\fb")]
    [InlineData("a\rb")]
    public void TakesNegativeResultOnInvalidName(string name)
    {
        IBool condition = new ValidConditionNameCondition(new String(name));

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnNameOfMaximumLength()
    {
        IBool condition = new ValidConditionNameCondition(
            new String(new string('a', 50))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnNameLongerThanMaximum()
    {
        IBool condition = new ValidConditionNameCondition(
            new String(new string('a', 51))
        );

        Assert.False(condition.BoolValue);
    }
}
