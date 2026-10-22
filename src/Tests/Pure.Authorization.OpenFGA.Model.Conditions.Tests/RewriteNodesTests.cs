using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record RewriteNodesTests
{
    [Fact]
    public void ProducesEveryNodeOfTree()
    {
        FakeDirect direct = new(new FakeTypeForm("user"));
        FakeComputed owner = new("owner");
        FakeFrom parent = new("viewer", "parent");
        FakeComputed blocked = new("blocked");
        FakeIntersection intersection = new(owner, parent);
        FakeDifference difference = new(intersection, blocked);
        FakeUnion union = new(direct, difference);

        IEnumerable<IRewrite> nodes = new RewriteNodes(union);

        Assert.Equal(
            [union, direct, difference, intersection, owner, parent, blocked],
            nodes
        );
    }

    [Fact]
    public void ThrowsExceptionOnUnknownNode()
    {
        IEnumerable<IRewrite> nodes = new RewriteNodes(
            new FakeUnion(new FakeComputed("owner"), new UnknownRewrite())
        );

        _ = Assert.Throws<ArgumentException>(nodes.ToArray);
    }
}
