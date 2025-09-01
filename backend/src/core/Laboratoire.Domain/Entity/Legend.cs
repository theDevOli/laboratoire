using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Laboratoire.Domain.Entity;

public class Legend
{
    [JsonPropertyName("unit")]
    [Required]
    public string? Unit { get; set; }
    [Required]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

}
