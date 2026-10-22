using Pure.Primitives.Abstractions.Bool;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidRelationNameConditionTests
{
    [Theory]
    [InlineData("viewer")]
    [InlineData("a")]
    [InlineData("can_read-2.x")]
    [InlineData("a*b")]
    [InlineData("a\vb")]
    [InlineData("This")]
    public void TakesPositiveResultOnValidName(string name)
    {
        IBool condition = new ValidRelationNameCondition(new String(name));

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
        IBool condition = new ValidRelationNameCondition(new String(name));

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnNameOfMaximumLength()
    {
        IBool condition = new ValidRelationNameCondition(new String(new string('a', 50)));

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnNameLongerThanMaximum()
    {
        IBool condition = new ValidRelationNameCondition(new String(new string('a', 51)));

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void CountsLengthInCodePoints()
    {
        IBool condition = new ValidRelationNameCondition(
            new String(string.Concat(Enumerable.Repeat("\U0001F600", 50)))
        );

        Assert.True(condition.BoolValue);
    }
}
