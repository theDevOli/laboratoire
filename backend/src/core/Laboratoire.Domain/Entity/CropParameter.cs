using System.Text.Json.Serialization;

namespace Laboratoire.Domain.Entity;

public class CropParameter
{
    private readonly int _maximumReference = 20;
    private readonly int _mediumReference = 10;
    [JsonPropertyName("min")]
    public int Min { get; set; }
    [JsonPropertyName("med")]
    public int Med { get; set; }
    [JsonPropertyName("max")]
    public int Max { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        CropParameter other = (CropParameter)obj!;

        return other.Min == this.Min
            && other.Med == this.Med
            && other.Max == this.Max;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Med, Max);
    }

    // TODO:Implement latter
    // public void GetValue(this CropParameter parameter, double? result)
    // {
    //     if (!result.HasValue) return;

    //     int? test = result > _maximumReference ? parameter.Max
    //              : result > _mediumReference ? parameter.Med
    //              : parameter?.Min;
    // }
}
