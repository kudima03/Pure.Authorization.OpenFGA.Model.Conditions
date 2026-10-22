using Pure.Primitives.Abstractions.Bool;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidTypeNameConditionTests
{
    [Theory]
    [InlineData("document")]
    [InlineData("a")]
    [InlineData("user_v2-x.y")]
    [InlineData("a*b")]
    [InlineData("a\vb")]
    [InlineData("a b")]
    [InlineData("Self")]
    [InlineData("selfie")]
    public void TakesPositiveResultOnValidName(string name)
    {
        IBool condition = new ValidTypeNameCondition(new String(name));

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
    [InlineData("self")]
    [InlineData("this")]
    public void TakesNegativeResultOnInvalidName(string name)
    {
        IBool condition = new ValidTypeNameCondition(new String(name));

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnNameOfMaximumLength()
    {
        IBool condition = new ValidTypeNameCondition(new String(new string('a', 254)));

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnNameLongerThanMaximum()
    {
        IBool condition = new ValidTypeNameCondition(new String(new string('a', 255)));

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void CountsLengthInCodePoints()
    {
        IBool condition = new ValidTypeNameCondition(
            new String(string.Concat(Enumerable.Repeat("\U0001F600", 254)))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnCodePointsLongerThanMaximum()
    {
        IBool condition = new ValidTypeNameCondition(
            new String(string.Concat(Enumerable.Repeat("\U0001F600", 255)))
        );

        Assert.False(condition.BoolValue);
    }
}
