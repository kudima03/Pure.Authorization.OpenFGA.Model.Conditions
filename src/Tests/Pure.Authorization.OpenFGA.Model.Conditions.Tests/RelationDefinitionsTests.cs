using System.Collections;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record RelationDefinitionsTests
{
    private static readonly FakeRelation Owner = new("owner", new FakeComputed("editor"));

    private static readonly FakeType Document = new(
        "document",
        new FakeRelation("editor", new FakeDirect(new FakeTypeForm("user"))),
        Owner
    );

    [Fact]
    public void ProducesRelationsOfNameOnTypesOfModel()
    {
        IEnumerable<IRelationDefinition> relations = new RelationDefinitions(
            new FakeModel(new FakeType("user"), Document),
            new String("document"),
            new String("owner")
        );

        Assert.Equal([Owner], relations);
    }

    [Fact]
    public void ProducesRelationsOfNameOnType()
    {
        IEnumerable<IRelationDefinition> relations = new RelationDefinitions(
            Document,
            new String("owner")
        );

        Assert.Equal([Owner], relations);
    }

    [Fact]
    public void ProducesRelationsOfNameThroughNonGenericEnumerator()
    {
        IEnumerable relations = new RelationDefinitions(Document, new String("owner"));

        Assert.Equal([Owner], new NonGenericItems(relations));
    }
}
