using Pure.Primitives.Abstractions.Bool;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record SupportedSchemaVersionConditionTests
{
    [Theory]
    [InlineData("1.1")]
    [InlineData("1.2")]
    public void TakesPositiveResultOnSupportedVersion(string version)
    {
        IBool condition = new SupportedSchemaVersionCondition(new String(version));

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("1.3")]
    [InlineData("2.1")]
    [InlineData("1.1 ")]
    [InlineData("")]
    public void TakesNegativeResultOnUnsupportedVersion(string version)
    {
        IBool condition = new SupportedSchemaVersionCondition(new String(version));

        Assert.False(condition.BoolValue);
    }
}
