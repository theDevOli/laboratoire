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

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;
        Legend? other = obj as Legend;

        return other?.Unit == this.Unit && other?.Description == this.Description;
    }

    public override int GetHashCode()
    => HashCode.Combine(this.Unit, this.Description);

}
