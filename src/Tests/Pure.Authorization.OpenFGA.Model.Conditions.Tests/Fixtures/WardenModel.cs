using Pure.Authorization.OpenFGA.Model.Abstractions.Condition;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Fixtures;

/// <summary>
/// The Warden authorization model (<c>Warden.Infrastructure/OpenFga/model.fga</c>),
/// translated relation by relation into test doubles.
/// </summary>
internal sealed record WardenModel : IAuthorizationModel
{
    private readonly IAuthorizationModel _model = new FakeModel(
        new FakeType("user"),
        new FakeType("client"),
        new FakeType(
            "organization",
            new FakeRelation(
                "admin",
                new FakeDirect(
                    new FakeTypeForm("user"),
                    new FakeSubjectSetForm("group", "member")
                )
            ),
            new FakeRelation(
                "member",
                new FakeUnion(
                    new FakeDirect(
                        new FakeTypeForm("user"),
                        new FakeSubjectSetForm("group", "member")
                    ),
                    new FakeComputed("admin")
                )
            )
        ),
        new FakeType(
            "group",
            new FakeRelation(
                "organization",
                new FakeDirect(new FakeTypeForm("organization"))
            ),
            new FakeRelation("admin", new FakeDirect(new FakeTypeForm("user"))),
            new FakeRelation(
                "member",
                new FakeDirect(
                    new FakeTypeForm("user"),
                    new FakeSubjectSetForm("group", "member")
                )
            ),
            new FakeRelation(
                "can_manage",
                new FakeUnion(
                    new FakeComputed("admin"),
                    new FakeFrom("admin", "organization")
                )
            )
        ),
        new FakeType(
            "datasource",
            new FakeRelation(
                "organization",
                new FakeDirect(new FakeTypeForm("organization"))
            ),
            new FakeRelation("ingester", new FakeDirect(new FakeTypeForm("client"))),
            new FakeRelation(
                "steward",
                new FakeUnion(
                    new FakeDirect(
                        new FakeTypeForm("user"),
                        new FakeSubjectSetForm("group", "member")
                    ),
                    new FakeFrom("admin", "organization")
                )
            ),
            new FakeRelation(
                "reader",
                new FakeUnion(ReaderDirect(), new FakeComputed("steward"))
            ),
            new FakeRelation("can_grant", new FakeComputed("steward"))
        ),
        new FakeType(
            "schema",
            new FakeRelation("parent", new FakeDirect(new FakeTypeForm("datasource"))),
            StewardFromParent(),
            new FakeRelation(
                "reader",
                new FakeUnion(
                    ReaderDirect(),
                    new FakeFrom("reader", "parent"),
                    new FakeComputed("steward")
                )
            ),
            new FakeRelation("can_grant", new FakeComputed("steward"))
        ),
        new FakeType(
            "table",
            new FakeRelation("parent", new FakeDirect(new FakeTypeForm("schema"))),
            StewardFromParent(),
            new FakeRelation(
                "reader",
                new FakeUnion(
                    ReaderDirect(),
                    new FakeFrom("reader", "parent"),
                    new FakeComputed("steward")
                )
            ),
            new FakeRelation(
                "blocked",
                new FakeDirect(
                    new FakeTypeForm("user"),
                    new FakeSubjectSetForm("group", "member")
                )
            ),
            new FakeRelation(
                "can_read",
                new FakeDifference(
                    new FakeComputed("reader"),
                    new FakeComputed("blocked")
                )
            ),
            new FakeRelation("bypass_row_policies", new FakeComputed("steward")),
            new FakeRelation("can_grant", new FakeComputed("steward"))
        ),
        new FakeType(
            "column",
            new FakeRelation("parent", new FakeDirect(new FakeTypeForm("table"))),
            new FakeRelation("steward", new FakeFrom("steward", "parent")),
            new FakeRelation(
                "reader",
                new FakeUnion(ReaderDirect(), new FakeFrom("reader", "parent"))
            ),
            new FakeRelation(
                "blocked",
                new FakeUnion(
                    new FakeDirect(
                        new FakeTypeForm("user"),
                        new FakeSubjectSetForm("group", "member")
                    ),
                    new FakeFrom("blocked", "parent")
                )
            ),
            new FakeRelation(
                "can_read",
                new FakeDifference(
                    new FakeComputed("reader"),
                    new FakeComputed("blocked")
                )
            ),
            new FakeRelation("can_grant", new FakeComputed("steward"))
        ),
        new FakeType(
            "folder",
            new FakeRelation(
                "organization",
                new FakeDirect(new FakeTypeForm("organization"))
            ),
            new FakeRelation("parent", new FakeDirect(new FakeTypeForm("folder"))),
            new FakeRelation("owner", new FakeDirect(new FakeTypeForm("user"))),
            Editor("parent"),
            Viewer("parent"),
            new FakeRelation(
                "can_grant",
                new FakeUnion(new FakeComputed("owner"), new FakeComputed("editor"))
            )
        ),
        Visualization("chart"),
        Visualization("diagram"),
        new FakeType(
            "query",
            new FakeRelation(
                "visualization",
                new FakeDirect(new FakeTypeForm("chart"), new FakeTypeForm("diagram"))
            ),
            new FakeRelation("editor", new FakeFrom("editor", "visualization")),
            new FakeRelation("viewer", new FakeFrom("viewer", "visualization"))
        ),
        new FakeType(
            "workspace",
            new FakeRelation("owner", new FakeDirect(new FakeTypeForm("user"))),
            new FakeRelation(
                "editor",
                new FakeUnion(
                    new FakeDirect(
                        new FakeTypeForm("user"),
                        new FakeSubjectSetForm("group", "member")
                    ),
                    new FakeComputed("owner")
                )
            ),
            new FakeRelation(
                "viewer",
                new FakeUnion(ReaderDirect(), new FakeComputed("editor"))
            ),
            new FakeRelation("can_grant", new FakeComputed("owner")),
            new FakeRelation("can_delete", new FakeComputed("owner"))
        ),
        new FakeType(
            "widget",
            new FakeRelation("workspace", new FakeDirect(new FakeTypeForm("workspace"))),
            new FakeRelation("editor", new FakeFrom("editor", "workspace")),
            new FakeRelation("viewer", new FakeFrom("viewer", "workspace"))
        )
    );

    public IString SchemaVersion => _model.SchemaVersion;

    public IEnumerable<ITypeDefinition> Types => _model.Types;

    public IEnumerable<ICondition> Conditions => _model.Conditions;

    // [user, group#member, organization#member]
    private static FakeDirect ReaderDirect()
    {
        return new FakeDirect(
            new FakeTypeForm("user"),
            new FakeSubjectSetForm("group", "member"),
            new FakeSubjectSetForm("organization", "member")
        );
    }

    // define steward: [user, group#member] or steward from parent
    private static FakeRelation StewardFromParent()
    {
        return new FakeRelation(
            "steward",
            new FakeUnion(
                new FakeDirect(
                    new FakeTypeForm("user"),
                    new FakeSubjectSetForm("group", "member")
                ),
                new FakeFrom("steward", "parent")
            )
        );
    }

    // define editor: [user, group#member] or owner or editor from <tupleset>
    private static FakeRelation Editor(string tupleset)
    {
        return new FakeRelation(
            "editor",
            new FakeUnion(
                new FakeDirect(
                    new FakeTypeForm("user"),
                    new FakeSubjectSetForm("group", "member")
                ),
                new FakeComputed("owner"),
                new FakeFrom("editor", tupleset)
            )
        );
    }

    // define viewer: [user, group#member, organization#member] or editor or viewer from <tupleset>
    private static FakeRelation Viewer(string tupleset)
    {
        return new FakeRelation(
            "viewer",
            new FakeUnion(
                ReaderDirect(),
                new FakeComputed("editor"),
                new FakeFrom("viewer", tupleset)
            )
        );
    }

    // type chart / type diagram
    private static FakeType Visualization(string name)
    {
        return new FakeType(
            name,
            new FakeRelation("folder", new FakeDirect(new FakeTypeForm("folder"))),
            new FakeRelation("owner", new FakeDirect(new FakeTypeForm("user"))),
            Editor("folder"),
            Viewer("folder"),
            new FakeRelation(
                "can_grant",
                new FakeUnion(new FakeComputed("owner"), new FakeComputed("editor"))
            ),
            new FakeRelation("can_delete", new FakeComputed("owner"))
        );
    }
}
