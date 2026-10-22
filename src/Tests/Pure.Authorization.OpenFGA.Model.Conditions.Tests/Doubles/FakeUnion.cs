using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeUnion : IUnion
{
    public FakeUnion(params IEnumerable<IRewrite> children)
    {
        Children = children;
    }

    public IEnumerable<IRewrite> Children { get; }
}
