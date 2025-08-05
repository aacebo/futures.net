using System.Text.Json.Serialization;

namespace Samples.Functions;

public class Params
{
    [JsonPropertyName("value")]
    public int Value { get; init; }
}