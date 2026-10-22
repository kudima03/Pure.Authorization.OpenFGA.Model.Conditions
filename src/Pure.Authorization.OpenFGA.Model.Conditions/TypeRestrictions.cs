using System.Collections;
using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// The subject forms a relation can be written for directly: the forms of every
/// <see cref="IDirect"/> in its rewrite, wherever it sits in the tree. OpenFGA keeps them
/// per relation as <c>directly_related_user_types</c>, regardless of where <c>this</c>
/// appears in the rewrite.
/// </summary>
internal sealed record TypeRestrictions : IEnumerable<ISubjectForm>
{
    private readonly IEnumerable<IRelationDefinition> _relations;

    public TypeRestrictions(IRelationDefinition relation)
        : this([relation]) { }

    public TypeRestrictions(IEnumerable<IRelationDefinition> relations)
    {
        _relations = relations;
    }

    public IEnumerator<ISubjectForm> GetEnumerator()
    {
        return _relations
            .SelectMany(relation => new RewriteNodes(relation.Rewrite))
            .OfType<IDirect>()
            .SelectMany(direct => direct.Forms)
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
