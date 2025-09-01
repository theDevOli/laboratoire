namespace Laboratoire.Domain.Entity;

public class FertilizerSuggestion
{
    public string CropName { get; set; } = default!;
    public double? CalagemTon { get; set; }
    public double? CalagemKg { get; set; }
    public int? NitrogenNecessity { get; set; }
    public int? PhosphorusNecessity { get; set; }
    public int? PotassiumNecessity { get; set; }
    // public IEnumerable<NPKFormulation> Formulation { get; set; } = default!;
    public NPKFormulation Base { get; set; } = default!;
    public NPKFormulation Cover { get; set; } = default!;

}

public class NPKFormulation
{
    public string Formulation { get; set; } = default!;
    public double? QuantityInHa { get; set; }
    public double? QuantityInTarefa { get; set; }

}