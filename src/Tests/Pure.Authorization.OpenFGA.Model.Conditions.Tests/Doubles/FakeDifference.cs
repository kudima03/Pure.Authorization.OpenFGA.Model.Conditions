using Pure.Authorization.OpenFGA.Model.Abstractions.Rewrite;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

internal sealed record FakeDifference : IDifference
{
    public FakeDifference(IRewrite @base, IRewrite subtract)
    {
        Base = @base;
        Subtract = subtract;
    }

    public IRewrite Base { get; }

    public IRewrite Subtract { get; }
}
