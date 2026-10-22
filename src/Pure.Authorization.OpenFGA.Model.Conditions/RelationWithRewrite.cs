using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// A relation seen through one node of its rewrite: the name of the relation, with the
/// rewrite replaced by that node. Lets a rewrite be checked node by node against the
/// relation it defines.
/// </summary>
internal sealed record RelationWithRewrite : IRelationDefinition
{
    private readonly IRelationDefinition _relation;

    public RelationWithRewrite(IRelationDefinition relation, IRewrite rewrite)
    {
        _relation = relation;
        Rewrite = rewrite;
    }

    public IString Name => _relation.Name;

    public IRewrite Rewrite { get; }
}
