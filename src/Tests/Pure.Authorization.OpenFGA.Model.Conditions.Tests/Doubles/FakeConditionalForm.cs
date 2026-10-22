using Pure.Authorization.OpenFGA.Model.Abstractions.SubjectForm;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeConditionalForm : IConditionalForm
{
    public FakeConditionalForm(ISubjectForm form, string condition)
    {
        Form = form;
        Condition = new String(condition);
    }

    public ISubjectForm Form { get; }

    public IString Condition { get; }
}
