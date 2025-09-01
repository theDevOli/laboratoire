using System.Text.Json.Serialization;

namespace Laboratoire.Domain.Entity;

public class CropParameter
{

    [JsonPropertyName("min")]
    public int Min { get; set; }
    [JsonPropertyName("med")]
    public int Med { get; set; }
    [JsonPropertyName("max")]
    public int Max { get; set; }
}
