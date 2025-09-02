using System.Text.Json;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class ReportMapper
{
    public static Report ToReport(this ReportDtoAdd dto)
    => new Report()
    {
        ReportId = default,
        Results = dto?.Results
    };
    public static ReportPatch ToReportPatch(this ReportDtoAdd dto,Guid? reportId)
    => new ReportPatch()
    {
        ReportId = reportId,
        ProtocolId = dto?.ProtocolId,
        Results = dto?.Results
    };

    public static Report FromDb(this ReportDtoDb dto)
    => new Report()
    {
        ReportId = dto.ReportId,
        Results = dto.Results?
        .Select(res => JsonSerializer.Deserialize<ReportResult>(res))
        .ToArray()!
    };
    public static ReportDtoDb ToDb(this Report report)
    => new ReportDtoDb()
    {
        ReportId = report.ReportId,
        Results = report.Results?
        .Select(res => JsonSerializer.Serialize(res))
        .ToArray()
    };

    public static ReportPDF FromDb(this ReportDtoPDFDb dto)
    {
        var pdf = new ReportPDF()
        {
            ProtocolId = dto?.ProtocolId,
            Results = dto?.Results?
            .Select(result => JsonSerializer.Deserialize<ReportResult>(result))
            .ToArray()!,
            ClientName = dto?.ClientName,
            ClientTaxId = dto?.ClientTaxId,
            ClientEmail = dto?.ClientEmail,
            ClientPhone = dto?.ClientPhone,
            Registration = dto?.Registration,
            PropertyName = dto?.PropertyName,
            Location = dto?.Location,
            Area = dto?.Area,
            Ccir = dto?.Ccir,
            ItrNirf = dto?.ItrNirf,
            CatalogId = dto?.CatalogId,
            ReportType = dto?.ReportType,
            SampleType = dto?.SampleType,
            Outputs = default,
            EntryDate = dto?.EntryDate,
            ReportDate = dto?.ReportDate,
            Legends = dto?.Legends?
                .Select(legend => JsonSerializer.Deserialize<Legend>(legend))
                .ToArray()!,
            IsCollectedByClient = dto?.IsCollectedByClient,
            QRCode = default,
            Suggestions = default
        };

        return pdf;
    }
    public static Report ToReport(this ReportPatch dto)
    => new Report()
    {
        ReportId = dto.ReportId,
        Results = dto?.Results
    };
}