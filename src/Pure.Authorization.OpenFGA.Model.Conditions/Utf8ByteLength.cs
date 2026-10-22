using System.Text;
using Pure.Primitives.Abstractions.Number;
using Pure.Primitives.Abstractions.String;

namespace Pure.Authorization.OpenFGA.Model.Conditions;

/// <summary>
/// The number of bytes of a text encoded as UTF-8, the unit of OpenFGA's <c>max_bytes</c>
/// validation rules.
/// </summary>
internal sealed record Utf8ByteLength : INumber<int>
{
    private readonly IString _text;

    public Utf8ByteLength(IString text)
    {
        _text = text;
    }

    public int NumberValue => Encoding.UTF8.GetByteCount(_text.TextValue);
}
