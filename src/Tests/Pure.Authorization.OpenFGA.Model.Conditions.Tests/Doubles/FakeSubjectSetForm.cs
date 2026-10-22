using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeSubjectSetForm : ISubjectSetForm
{
    public FakeSubjectSetForm(string type, string relation)
    {
        Type = new String(type);
        Relation = new String(relation);
    }

    public IString Type { get; }

    public IString Relation { get; }
}
