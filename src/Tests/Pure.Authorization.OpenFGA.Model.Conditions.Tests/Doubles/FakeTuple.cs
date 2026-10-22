using Pure.Authorization.OpenFGA.Model.Abstractions.Object;
using Pure.Authorization.OpenFGA.Model.Abstractions.Subject;
using Pure.Authorization.OpenFGA.Model.Abstractions.Tuple;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeTuple : ITuple
{
    public FakeTuple(ISubject subject, string relation, IObject @object)
    {
        Subject = subject;
        Relation = new String(relation);
        Object = @object;
    }

    public ISubject Subject { get; }

    public IString Relation { get; }

    public IObject Object { get; }
}
