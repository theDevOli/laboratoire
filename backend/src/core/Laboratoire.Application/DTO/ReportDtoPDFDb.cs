using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Application.DTO;
public sealed class ReportDtoPDFDb
{
    [Required]
    public string? ProtocolId { get; set; }
    [Required]
    public string[]? Results { get; set; }
    [Required]
    public string? ClientName { get; set; }
    [Required]
    public string? ClientTaxId { get; set; }
    [EmailAddress]
    public string? ClientEmail { get; set; }
    [RegularExpression(@"^(\(?\d{2}\)?\s?)?\d{5}-?\d{4}$")]
    public string? Registration { get; set; }
    public string? ClientPhone { get; set; }
    public string? StateCode { get; set; }
    [Required]
    public string? PropertyName { get; set; }
    [Required]
    public string? City { get; set; }

    public string? Location
    {
        get
        {
            return $"{City} - {StateCode}, {PropertyName}";
        }
    }
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
    public DateTime? EntryDate { get; set; }
    [Required]
    public DateTime? ReportDate { get; set; }
    [Required]
    public string[]? Legends { get; set; }
    [Required]
    public bool? IsCollectedByClient { get; set; }
}