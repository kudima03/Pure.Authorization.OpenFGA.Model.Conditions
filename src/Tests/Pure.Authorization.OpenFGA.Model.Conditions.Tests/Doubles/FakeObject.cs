using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeObject : IObject
{
    public FakeObject(string type, string id)
    {
        Type = new String(type);
        Id = new String(id);
    }

    public IString Type { get; }

    public IString Id { get; }
}
