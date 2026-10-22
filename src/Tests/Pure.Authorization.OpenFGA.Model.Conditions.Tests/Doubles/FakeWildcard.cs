using Pure.Authorization.OpenFGA.Model.Abstractions.Subject;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeWildcard : ITypedWildcard
{
    public FakeWildcard(string type)
    {
        Type = new String(type);
    }

    public IString Type { get; }
}
