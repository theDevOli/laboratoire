using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Globalization;

namespace Laboratoire.Domain.Entity;

public class ReportResult
{
    [JsonPropertyName("parameterId")]
    [Required]
    public int? ParameterId { get; set; }
    [Required]
    [JsonPropertyName("valueA")]
    public double? ValueA { get; set; }
    [JsonPropertyName("valueB")]
    public double? ValueB { get; set; }
    [JsonPropertyName("valueC")]
    public double? ValueC { get; set; }

    [JsonPropertyName("equation")]
    public string? Equation { get; set; }
    public override string ToString()
    {
        if (ValueA is null)
            return "ND";

        if (ValueA == 0)
            return "ND";

        if (ValueB is not null)
            return (ValueB - ValueA)?.ToString("F2", new CultureInfo("pt-BR"))!;

        return ValueA?.ToString("F2", new CultureInfo("pt-BR"))!;
    }

    public double GetResult()
    {
        if (ValueB is not null)
            return ValueB - ValueA ?? 0;

        return ValueA ?? 0;
    }
}
