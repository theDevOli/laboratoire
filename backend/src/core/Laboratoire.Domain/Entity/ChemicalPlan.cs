using System.Text.Json.Serialization;

namespace Laboratoire.Domain.Entity;

public class ChemicalPlan
{
    [JsonPropertyName("solutionID")]
    public int SolutionID { get; set; }

    [JsonPropertyName("quantityPerAnalysis")]
    public double QuantityPerAnalysis { get; set; }

    [JsonPropertyName("quantityPerSolution")]
    public double QuantityPerSolution { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != this.GetType())
            return false;

        ChemicalPlan other = (ChemicalPlan)obj;

        return this.SolutionID == other.SolutionID
        && this.QuantityPerAnalysis == other.QuantityPerAnalysis
        && this.QuantityPerSolution == other.QuantityPerSolution;
    }

    public override int GetHashCode()
    => HashCode.Combine(SolutionID, QuantityPerAnalysis, QuantityPerSolution);

}
