using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Laboratoire.Domain.Entity;

public class ReportPDF
{
    private string _emptyData = string.Concat(Enumerable.Repeat("*", 6));
    [Required]
    public string? ProtocolId { get; set; }
    [Required]
    public ReportResult[]? Results { get; set; }
    [Required]
    public string? ClientName { get; set; }
    [Required]
    public string? ClientTaxId { get; set; }
    [EmailAddress]
    public string? ClientEmail { get; set; }
    [RegularExpression(@"^(\(?\d{2}\)?\s?)?\d{5}-?\d{4}$")]
    public string? ClientPhone { get; set; }
    [Required]
    public string? PropertyName { get; set; }
    public string? Registration { get; set; }
    public string? Location { get; set; }
    public string? Area { get; set; }
    public string? Ccir { get; set; }
    public string? ItrNirf { get; set; }
    [Required]
    public int? CatalogId { get; set; }
    [Required]
    public string? ReportType { get; set; }
    [Required]
    public string? SampleType { get; set; }
    [Required]
    public IEnumerable<TableOutput>? Outputs { get; set; }
    [Required]
    public DateTime? EntryDate { get; set; }
    [Required]
    public DateTime? ReportDate { get; set; }
    [Required]
    public Legend[]? Legends { get; set; }
    [Required]
    public bool? IsCollectedByClient { get; set; }
    public string? QRCode { get; set; }
    // public IEnumerable<Crop>? Crops { get; set; }
    public IEnumerable<FertilizerSuggestion>? Suggestions { get; set; }
    public string ToTaxId()
    {
        if (ClientTaxId == null || ClientTaxId.Length < 11)
            return _emptyData;

        if (ClientTaxId.Length == 11)
            return $"{ClientTaxId.Substring(0, 3)}.{ClientTaxId.Substring(3, 3)}.{ClientTaxId.Substring(6, 3)}-{ClientTaxId.Substring(9, 2)}";

        if (ClientTaxId.Length == 14)
            return $"{ClientTaxId.Substring(0, 2)}.{ClientTaxId.Substring(2, 3)}.{ClientTaxId.Substring(5, 3)}/{ClientTaxId.Substring(8, 4)}-{ClientTaxId.Substring(12, 2)}";

        return _emptyData;
    }

    public string ToPhone()
    {
        if (ClientPhone is null)
            return _emptyData;

        return $"({ClientPhone.Substring(0, 2)}) {ClientPhone.Substring(2, 1)} {ClientPhone.Substring(3, 4)}-{ClientPhone.Substring(7, 4)}";
    }

    public string ToCcir()
    {
        if (Ccir is null)
            return _emptyData;

        var digits = Regex.Replace(Ccir, @"\D", "");

        return digits.Length switch
        {
            <= 3 => Regex.Replace(Ccir, @"(\d{3})", "$1"),
            <= 6 => Regex.Replace(Ccir, @"(\d{3})(\d{3})", "$1.$2"),
            <= 9 => Regex.Replace(Ccir, @"(\d{3})(\d{3})(\d{3})", "$1.$2.$3"),
            <= 11 => Regex.Replace(Ccir, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4"),
            _ => Regex.Replace(Ccir, @"(\d{3})(\d{3})(\d{3})(\d{3})(\d{1})", "$1.$2.$3.$4-$5")
        };
    }
    public string ToItrNirf()
    {
        if (ItrNirf is null)
            return _emptyData;
        return Regex.Replace(ItrNirf, @"(\d{1})(\d{3})(\d{3})(\d{1})", "$1.$2.$3-$4");
    }
}
