using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeType : ITypeDefinition
{
    public FakeType(string name, params IEnumerable<IRelationDefinition> relations)
        : this(new String(name), relations) { }

    public FakeType(IString name, IEnumerable<IRelationDefinition> relations)
    {
        Name = name;
        Relations = relations;
    }

    public IString Name { get; }

    public IEnumerable<IRelationDefinition> Relations { get; }
}
