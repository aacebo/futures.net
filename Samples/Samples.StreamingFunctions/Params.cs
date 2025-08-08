using System.Text.Json.Serialization;

namespace Samples.StreamingFunctions;

public class Params
{
    [JsonPropertyName("value")]
    public int Value { get; init; }
}