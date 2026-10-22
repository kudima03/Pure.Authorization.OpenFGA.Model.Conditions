using Pure.Authorization.OpenFGA.Model.Abstractions.Model;
using Pure.Primitives.Abstractions.String;
using String = Pure.Primitives.String.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions.Tests.Doubles;

/// <summary>
/// The first type of the model with the given name.
/// </summary>
internal sealed record ModelType : ITypeDefinition
{
    private readonly IAuthorizationModel _model;

    public ModelType(IAuthorizationModel model, string name)
    {
        _model = model;
        Name = new String(name);
    }

    public IString Name { get; }

    public IEnumerable<IRelationDefinition> Relations =>
        _model.Types.First(type => type.Name.TextValue == Name.TextValue).Relations;
}
