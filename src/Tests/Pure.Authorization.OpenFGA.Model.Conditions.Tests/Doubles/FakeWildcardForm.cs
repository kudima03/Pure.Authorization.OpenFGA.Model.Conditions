using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeWildcardForm : IWildcardForm
{
    public FakeWildcardForm(string type)
    {
        Type = new String(type);
    }

    public IString Type { get; }
}
