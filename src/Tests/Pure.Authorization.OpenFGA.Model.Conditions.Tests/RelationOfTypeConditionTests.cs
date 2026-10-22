using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.Bool;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record RelationOfTypeConditionTests
{
    private static readonly IAuthorizationModel Model = new FakeModel(
        new FakeType("user"),
        new FakeType(
            "document",
            new FakeRelation("owner", new FakeDirect(new FakeTypeForm("user"))),
            new FakeRelation("viewer", new FakeComputed("owner"))
        ),
        new FakeType(
            "folder",
            new FakeRelation("editor", new FakeDirect(new FakeTypeForm("user")))
        )
    );

    [Theory]
    [InlineData("document", "owner")]
    [InlineData("document", "viewer")]
    [InlineData("folder", "editor")]
    public void TakesPositiveResultOnRelationOfType(string type, string relation)
    {
        IBool condition = new RelationOfTypeCondition(
            Model,
            new String(type),
            new String(relation)
        );

        Assert.True(condition.BoolValue);
    }

    [Theory]
    [InlineData("document", "editor")]
    [InlineData("folder", "owner")]
    [InlineData("user", "owner")]
    [InlineData("group", "owner")]
    [InlineData("document", "Owner")]
    public void TakesNegativeResultOnRelationNotOfType(string type, string relation)
    {
        IBool condition = new RelationOfTypeCondition(
            Model,
            new String(type),
            new String(relation)
        );

        Assert.False(condition.BoolValue);
    }
}
