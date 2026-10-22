using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Authorization.OpenFGA.Model.Abstractions.Subject;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeSubjectSet : ISubjectSet
{
    public FakeSubjectSet(string type, string id, string relation)
    {
        Object = new FakeObject(type, id);
        Relation = new String(relation);
    }

    public IObject Object { get; }

    public IString Relation { get; }
}
