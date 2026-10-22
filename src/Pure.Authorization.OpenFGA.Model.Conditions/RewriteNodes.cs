using System.Collections;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// Every node of a rewrite tree, the root included. Throws
/// <see cref="ArgumentException"/> on enumeration when the tree holds an
/// <see cref="IRewrite"/> that is none of the known variants.
/// </summary>
internal sealed record RewriteNodes : IEnumerable<IRewrite>
{
    private readonly IRewrite _rewrite;

    public RewriteNodes(IRewrite rewrite)
    {
        _rewrite = rewrite;
    }

    public IEnumerator<IRewrite> GetEnumerator()
    {
        IEnumerable<IRewrite> operands = _rewrite switch
        {
            IDirect or IComputedRelation or IRelationFrom => [],
            IUnion union => union.Children,
            IIntersection intersection => intersection.Children,
            IDifference difference => [difference.Base, difference.Subtract],
            _ => throw new ArgumentException(),
        };

        IEnumerable<IRewrite> nodes =
        [
            _rewrite,
            .. operands.SelectMany(operand => new RewriteNodes(operand)),
        ];

        return nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
