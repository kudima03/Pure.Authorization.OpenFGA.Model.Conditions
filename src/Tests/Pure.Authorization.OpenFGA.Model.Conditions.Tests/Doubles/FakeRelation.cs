using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeRelation : IRelationDefinition
{
    public FakeRelation(string name, IRewrite rewrite)
    {
        Name = new String(name);
        Rewrite = rewrite;
    }

    public IString Name { get; }

    public IRewrite Rewrite { get; }
}
