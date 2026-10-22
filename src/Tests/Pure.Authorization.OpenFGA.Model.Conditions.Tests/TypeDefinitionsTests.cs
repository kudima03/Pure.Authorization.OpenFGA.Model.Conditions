using System.Collections;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record TypeDefinitionsTests
{
    private static readonly FakeType First = new("user");

    private static readonly FakeType Second = new("user");

    private static readonly IAuthorizationModel Model = new FakeModel(
        First,
        new FakeType("document"),
        Second
    );

    [Fact]
    public void ProducesTypesOfName()
    {
        IEnumerable<ITypeDefinition> types = new TypeDefinitions(
            Model,
            new String("user")
        );

        Assert.Equal([First, Second], types);
    }

    [Fact]
    public void ProducesTypesOfNameThroughNonGenericEnumerator()
    {
        IEnumerable types = new TypeDefinitions(Model, new String("user"));

        Assert.Equal([First, Second], new NonGenericItems(types));
    }
}
