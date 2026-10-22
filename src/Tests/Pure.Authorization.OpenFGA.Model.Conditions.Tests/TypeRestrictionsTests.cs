using System.Collections;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record TypeRestrictionsTests
{
    private static readonly FakeTypeForm User = new("user");

    private static readonly FakeSubjectSetForm Members = new("group", "member");

    private static readonly FakeWildcardForm Everyone = new("user");

    private static readonly FakeRelation Viewer = new(
        "viewer",
        new FakeUnion(
            new FakeDirect(User),
            new FakeDifference(
                new FakeComputed("owner"),
                new FakeDirect(Members, Everyone)
            )
        )
    );

    [Fact]
    public void ProducesFormsOfEveryDirect()
    {
        IEnumerable<ISubjectForm> forms = new TypeRestrictions(Viewer);

        Assert.Equal([User, Members, Everyone], forms);
    }

    [Fact]
    public void ProducesFormsThroughNonGenericEnumerator()
    {
        IEnumerable forms = new TypeRestrictions(Viewer);

        Assert.Equal([User, Members, Everyone], new NonGenericItems(forms));
    }
}
