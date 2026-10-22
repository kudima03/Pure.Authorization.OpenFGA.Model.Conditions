using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidSubjectConditionTests
{
    [Theory]
    [InlineData("user", "anne")]
    [InlineData("user", "anne@example.com")]
    [InlineData("a@b", "1")]
    [InlineData("a*", "1")]
    [InlineData("doc", "a*")]
    public void TakesPositiveResultOnValidObject(string type, string id)
    {
        IBool condition = new ValidSubjectCondition(new FakeObject(type, id));

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("", "anne")]
    [InlineData("user", "")]
    [InlineData("a:b", "1")]
    [InlineData("a#b", "1")]
    [InlineData("a b", "1")]
    [InlineData("a\u0001", "1")]
    [InlineData("user", "a:b")]
    [InlineData("user", "a#b")]
    [InlineData("user", "a b")]
    [InlineData("user", "a\tb")]
    [InlineData("user", "*")]
    public void TakesNegativeResultOnInvalidObject(string type, string id)
    {
        IBool condition = new ValidSubjectCondition(new FakeObject(type, id));

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnObjectOfMaximumBytes()
    {
        IBool condition = new ValidSubjectCondition(
            new FakeObject("u", new string('é', 255))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnObjectLongerThanMaximumBytes()
    {
        IBool condition = new ValidSubjectCondition(
            new FakeObject("u", new string('é', 256))
        );

        Assert.False(condition.BoolValue);
    }

    [Theory]
    [InlineData("group", "eng", "member")]
    [InlineData("a*", "eng", "member")]
    [InlineData("a@b", "eng", "member")]
    [InlineData("group", "eng@x", "member")]
    [InlineData("group", "eng", "a@b")]
    public void TakesPositiveResultOnValidSubjectSet(
        string type,
        string id,
        string relation
    )
    {
        IBool condition = new ValidSubjectCondition(
            new FakeSubjectSet(type, id, relation)
        );

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("", "eng", "member")]
    [InlineData("group", "", "member")]
    [InlineData("group", "eng", "")]
    [InlineData("a:b", "eng", "member")]
    [InlineData("a#b", "eng", "member")]
    [InlineData("a b", "eng", "member")]
    [InlineData("a\u0002", "eng", "member")]
    [InlineData("group", "a:b", "member")]
    [InlineData("group", "a#b", "member")]
    [InlineData("group", "a*b", "member")]
    [InlineData("group", "*", "member")]
    [InlineData("group", "a b", "member")]
    [InlineData("group", "a\rb", "member")]
    [InlineData("group", "eng", "a:b")]
    [InlineData("group", "eng", "a#b")]
    [InlineData("group", "eng", "a*b")]
    [InlineData("group", "eng", "a b")]
    [InlineData("group", "eng", "a\u009fb")]
    public void TakesNegativeResultOnInvalidSubjectSet(
        string type,
        string id,
        string relation
    )
    {
        IBool condition = new ValidSubjectCondition(
            new FakeSubjectSet(type, id, relation)
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnSubjectSetOfMaximumBytes()
    {
        IBool condition = new ValidSubjectCondition(
            new FakeSubjectSet("g", "1", new string('r', 508))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnSubjectSetLongerThanMaximumBytes()
    {
        IBool condition = new ValidSubjectCondition(
            new FakeSubjectSet("g", "1", new string('r', 509))
        );

        Assert.False(condition.BoolValue);
    }

    [Theory]
    [InlineData("user")]
    [InlineData("a*")]
    [InlineData("a@b")]
    public void TakesPositiveResultOnValidWildcard(string type)
    {
        IBool condition = new ValidSubjectCondition(new FakeWildcard(type));

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a:b")]
    [InlineData("a#b")]
    [InlineData("a b")]
    [InlineData("a\nb")]
    public void TakesNegativeResultOnInvalidWildcard(string type)
    {
        IBool condition = new ValidSubjectCondition(new FakeWildcard(type));

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnWildcardOfMaximumBytes()
    {
        IBool condition = new ValidSubjectCondition(
            new FakeWildcard(new string('t', 510))
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnWildcardLongerThanMaximumBytes()
    {
        IBool condition = new ValidSubjectCondition(
            new FakeWildcard(new string('t', 511))
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownSubject()
    {
        IBool condition = new ValidSubjectCondition(new UnknownSubject());

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }
}
