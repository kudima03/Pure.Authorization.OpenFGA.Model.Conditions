using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record UniqueRelationNamesConditionTests
{
    [Fact]
    public void TakesPositiveResultOnUniqueNames()
    {
        IBool condition = new UniqueRelationNamesCondition(
            new FakeType(
                "document",
                new FakeRelation("owner", new FakeDirect(new FakeTypeForm("user"))),
                new FakeRelation("viewer", new FakeComputed("owner"))
            )
        );

        Assert.True(condition.BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnDuplicateName()
    {
        IBool condition = new UniqueRelationNamesCondition(
            new FakeType(
                "document",
                new FakeRelation("owner", new FakeDirect(new FakeTypeForm("user"))),
                new FakeRelation("owner", new FakeComputed("viewer"))
            )
        );

        Assert.False(condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnTypeWithoutRelations()
    {
        IBool condition = new UniqueRelationNamesCondition(new FakeType("user"));

        Assert.True(condition.BoolValue);
    }
}
