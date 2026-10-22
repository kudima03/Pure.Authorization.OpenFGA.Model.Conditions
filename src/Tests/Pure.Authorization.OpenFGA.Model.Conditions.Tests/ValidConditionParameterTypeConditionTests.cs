using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ValidConditionParameterTypeConditionTests
{
    [Theory]
    [InlineData("any")]
    [InlineData("bool")]
    [InlineData("string")]
    [InlineData("int")]
    [InlineData("uint")]
    [InlineData("double")]
    [InlineData("duration")]
    [InlineData("timestamp")]
    [InlineData("ipaddress")]
    public void TakesPositiveResultOnNonGenericType(string name)
    {
        IBool condition = new ValidConditionParameterTypeCondition(
            new FakeParameterType(name)
        );

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("any")]
    [InlineData("string")]
    [InlineData("ipaddress")]
    public void TakesNegativeResultOnNonGenericTypeWithGenericType(string name)
    {
        IBool condition = new ValidConditionParameterTypeCondition(
            new FakeParameterType(name, new FakeParameterType("string"))
        );

        Assert.False(condition.BoolValue);
    }

    [Theory]
    [InlineData("map")]
    [InlineData("list")]
    public void TakesPositiveResultOnGenericTypeWithOneGenericType(string name)
    {
        IBool condition = new ValidConditionParameterTypeCondition(
            new FakeParameterType(name, new FakeParameterType("string"))
        );

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("map")]
    [InlineData("list")]
    public void TakesNegativeResultOnGenericTypeWithoutGenericType(string name)
    {
        IBool condition = new ValidConditionParameterTypeCondition(
            new FakeParameterType(name)
        );

        Assert.False(condition.BoolValue);
    }

    [Theory]
    [InlineData("map")]
    [InlineData("list")]
    public void TakesNegativeResultOnGenericTypeWithTwoGenericTypes(string name)
    {
        IBool condition = new ValidConditionParameterTypeCondition(
            new FakeParameterType(
                name,
                new FakeParameterType("string"),
                new FakeParameterType("int")
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnNestedGenericType()
    {
        IBool condition = new ValidConditionParameterTypeCondition(
            new FakeParameterType(
                "list",
                new FakeParameterType("map", new FakeParameterType("int"))
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnInvalidGenericType()
    {
        IBool condition = new ValidConditionParameterTypeCondition(
            new FakeParameterType("list", new FakeParameterType("map"))
        );

        Assert.False(condition.BoolValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData("String")]
    [InlineData("float")]
    [InlineData("TYPE_NAME_STRING")]
    public void TakesNegativeResultOnUnknownTypeName(string name)
    {
        IBool condition = new ValidConditionParameterTypeCondition(
            new FakeParameterType(name)
        );

        Assert.False(condition.BoolValue);
    }
}
