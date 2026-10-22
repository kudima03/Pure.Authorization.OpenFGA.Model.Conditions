using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;
using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeDirect : IDirect
{
    public FakeDirect(params IEnumerable<ISubjectForm> forms)
    {
        Forms = forms;
    }

    public IEnumerable<ISubjectForm> Forms { get; }
}
