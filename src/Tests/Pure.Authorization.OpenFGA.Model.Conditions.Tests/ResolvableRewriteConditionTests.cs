using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Fixtures;
using Pure.Primitives.Abstractions.Bool;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests;

public sealed record ResolvableRewriteConditionTests
{
    private static readonly FakeType Folder = new(
        "folder",
        new FakeRelation("owner", new FakeDirect(new FakeTypeForm("user"))),
        new FakeRelation("viewer", new FakeComputed("owner"))
    );

    private static readonly FakeType Team = new(
        "team",
        new FakeRelation("member", new FakeDirect(new FakeTypeForm("user")))
    );

    [Fact]
    public void TakesPositiveResultOnEveryRelationOfWardenModel()
    {
        IAuthorizationModel model = new WardenModel();

        Assert.All(
            model.Types.SelectMany(type =>
                type.Relations.Select(relation => new ResolvableRewriteCondition(
                    model,
                    type,
                    relation
                ))
            ),
            condition => Assert.True(condition.BoolValue)
        );
    }

    [Fact]
    public void TakesPositiveResultOnDirect()
    {
        Assert.True(Resolvable(new FakeDirect(new FakeTypeForm("user"))).BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnComputedRelationOfSameType()
    {
        Assert.True(Resolvable(new FakeComputed("owner")).BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnComputedRelationOfOtherType()
    {
        Assert.False(Resolvable(new FakeComputed("viewer")).BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnComputedRelationOfItself()
    {
        Assert.False(Resolvable(new FakeComputed("editor")).BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnRelationFromDirectTupleset()
    {
        Assert.True(Resolvable(new FakeFrom("viewer", "parent")).BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnRelationFromUndefinedTupleset()
    {
        Assert.False(Resolvable(new FakeFrom("viewer", "folder")).BoolValue);
    }

    [Fact]
    public void TakesNegativeResultOnRelationUndefinedOnTuplesetTypes()
    {
        Assert.False(Resolvable(new FakeFrom("member", "parent")).BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnRelationDefinedOnOneOfTuplesetTypes()
    {
        Assert.True(
            Resolvable(
                new FakeFrom("member", "parent"),
                new FakeDirect(new FakeTypeForm("folder"), new FakeTypeForm("team"))
            ).BoolValue
        );
    }

    [Fact]
    public void TakesPositiveResultOnConditionalTuplesetForm()
    {
        Assert.True(
            Resolvable(
                new FakeFrom("viewer", "parent"),
                new FakeDirect(new FakeConditionalForm(new FakeTypeForm("folder"), "c1"))
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetWithoutForms()
    {
        Assert.False(
            Resolvable(new FakeFrom("viewer", "parent"), new FakeDirect()).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetDefinedByRewrite()
    {
        Assert.False(
            Resolvable(
                new FakeFrom("viewer", "parent"),
                new FakeUnion(new FakeDirect(new FakeTypeForm("folder")))
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetAssignableFromWildcard()
    {
        Assert.False(
            Resolvable(
                new FakeFrom("viewer", "parent"),
                new FakeDirect(new FakeWildcardForm("folder"))
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetAssignableFromWildcardBesidesType()
    {
        Assert.False(
            Resolvable(
                new FakeFrom("viewer", "parent"),
                new FakeDirect(new FakeTypeForm("folder"), new FakeWildcardForm("team"))
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetAssignableFromSubjectSet()
    {
        Assert.False(
            Resolvable(
                new FakeFrom("viewer", "parent"),
                new FakeDirect(new FakeSubjectSetForm("folder", "owner"))
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnTuplesetAssignableFromConditionalWildcard()
    {
        Assert.False(
            Resolvable(
                new FakeFrom("viewer", "parent"),
                new FakeDirect(
                    new FakeConditionalForm(new FakeWildcardForm("folder"), "c1")
                )
            ).BoolValue
        );
    }

    [Fact]
    public void ThrowsExceptionOnUnknownTuplesetForm()
    {
        IBool condition = Resolvable(
            new FakeFrom("viewer", "parent"),
            new FakeDirect(new UnknownSubjectForm())
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownTuplesetFormBesidesType()
    {
        IBool condition = Resolvable(
            new FakeFrom("viewer", "parent"),
            new FakeDirect(new FakeTypeForm("folder"), new UnknownSubjectForm())
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void TakesPositiveResultOnUnionOfResolvableOperands()
    {
        Assert.True(
            Resolvable(
                new FakeUnion(
                    new FakeDirect(new FakeTypeForm("user")),
                    new FakeComputed("owner"),
                    new FakeFrom("viewer", "parent")
                )
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnUnionWithUnresolvableOperand()
    {
        Assert.False(
            Resolvable(
                new FakeUnion(new FakeComputed("owner"), new FakeComputed("viewer"))
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnUnionReferringToItself()
    {
        Assert.False(
            Resolvable(
                new FakeUnion(
                    new FakeDirect(new FakeTypeForm("user")),
                    new FakeComputed("editor")
                )
            ).BoolValue
        );
    }

    [Fact]
    public void TakesPositiveResultOnIntersectionOfResolvableOperands()
    {
        Assert.True(
            Resolvable(
                new FakeIntersection(
                    new FakeComputed("owner"),
                    new FakeFrom("viewer", "parent")
                )
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnIntersectionWithUnresolvableOperand()
    {
        Assert.False(
            Resolvable(
                new FakeIntersection(
                    new FakeComputed("owner"),
                    new FakeFrom("viewer", "team")
                )
            ).BoolValue
        );
    }

    [Fact]
    public void TakesPositiveResultOnDifferenceOfResolvableOperands()
    {
        Assert.True(
            Resolvable(
                new FakeDifference(
                    new FakeComputed("owner"),
                    new FakeFrom("viewer", "parent")
                )
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnDifferenceWithUnresolvableBase()
    {
        Assert.False(
            Resolvable(
                new FakeDifference(new FakeComputed("viewer"), new FakeComputed("owner"))
            ).BoolValue
        );
    }

    [Fact]
    public void TakesNegativeResultOnDifferenceWithUnresolvableSubtract()
    {
        Assert.False(
            Resolvable(
                new FakeDifference(new FakeComputed("owner"), new FakeComputed("viewer"))
            ).BoolValue
        );
    }

    [Fact]
    public void ThrowsExceptionOnUnknownRewrite()
    {
        IBool condition = Resolvable(new UnknownRewrite());

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    [Fact]
    public void ThrowsExceptionOnUnknownOperand()
    {
        IBool condition = Resolvable(
            new FakeUnion(new FakeComputed("owner"), new UnknownRewrite())
        );

        _ = Assert.Throws<ArgumentException>(() => condition.BoolValue);
    }

    // type document
    //   relations
    //     define owner: [user]
    //     define parent: <tupleset>
    //     define editor: <rewrite>
    private static ResolvableRewriteCondition Resolvable(IRewrite rewrite)
    {
        return Resolvable(rewrite, new FakeDirect(new FakeTypeForm("folder")));
    }

    private static ResolvableRewriteCondition Resolvable(
        IRewrite rewrite,
        IRewrite tupleset
    )
    {
        FakeRelation editor = new("editor", rewrite);
        FakeType document = new(
            "document",
            new FakeRelation("owner", new FakeDirect(new FakeTypeForm("user"))),
            new FakeRelation("parent", tupleset),
            editor
        );

        return new ResolvableRewriteCondition(
            new FakeModel(new FakeType("user"), Folder, Team, document),
            document,
            editor
        );
    }
}
