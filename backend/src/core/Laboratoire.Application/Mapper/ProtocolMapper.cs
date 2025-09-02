using System.Text.Json;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class ProtocolMapper
{
    public static Protocol ToProtocol(this ProtocolDtoAdd dto)
    => new Protocol()
    {
        ProtocolId = default,
        CashFlowId = default,
        ReportId = default,
        ClientId = dto.ClientId,
        PropertyId = dto.PropertyId,
        PartnerId = dto.PartnerId,
        CatalogId = dto.CatalogId,
        EntryDate = dto.EntryDate,
        ReportDate = dto.ReportDate,
        IsCollectedByClient = dto.IsCollectedByClient,
    };

    public static CropsNormalization[]? ToCropsNormalization(this ProtocolDtoAdd dto, string protocolId)
    {
        if (dto?.Crops is null)
            return null;

        return dto?.Crops?.Select(cropId => new CropsNormalization()
        {
            ProtocolId = protocolId,
            // EntryDate = dto.EntryDate,
            CropId = cropId
        }).ToArray()! ?? null;
    }

    public static CashFlow ToCashFlow(this ProtocolDtoAdd dto, string protocolId)
    => new CashFlow()
    {
        CashFlowId = default,
        Description = dto.TotalPaid > 0 ? $"Entrada de: {dto.DisplayMoney()}, protocolo: {protocolId}" : $"Saida de: {dto.DisplayMoney()}",
        TransactionId = dto.TransactionId,
        PartnerId = dto.PartnerId,
        TotalPaid = dto.TotalPaid,
        PaymentDate = dto.PaymentDate
    };

    public static CashFlow ToCashFlow(this ProtocolDtoUpdate dto)
    => new CashFlow()
    {
        CashFlowId = dto.CashFlowId,
        Description = dto.TotalPaid > 0 ? $"Entrada de: {dto.DisplayMoney()}, protocolo: {dto.ProtocolId}" : $"Saida de: {dto.DisplayMoney()}",
        TransactionId = dto.TransactionId,
        PartnerId = dto.PartnerId,
        TotalPaid = dto.TotalPaid == 0 ? null : dto.TotalPaid,
        PaymentDate = dto.PaymentDate
    };

    public static Protocol ToProtocol(this ProtocolDtoUpdate dto)
    => new Protocol()
    {
        ProtocolId = dto.ProtocolId,
        CashFlowId = dto.CashFlowId,
        ReportId = dto.ReportId,
        ClientId = dto.ClientId,
        PropertyId = dto.PropertyId,
        PartnerId = dto.PartnerId,
        CatalogId = dto.CatalogId,
        EntryDate = dto.EntryDate,
        ReportDate = dto.ReportDate,
        IsCollectedByClient = dto.IsCollectedByClient,
    };

    public static CropsNormalization[]? ToCropsNormalization(this ProtocolDtoUpdate dto)
    {
        if (dto?.Crops is null)
            return null;

        return dto?.Crops?.Select(cropId => new CropsNormalization()
        {
            ProtocolId = dto.ProtocolId,
            CropId = cropId
        }).ToArray()! ?? null;
    }

    public static IEnumerable<ProtocolDtoDisplay> ToProtocolDisplay(this IEnumerable<ProtocolDtoDisplayDb> dto, IEnumerable<CropsNormalization>? cropsNormalization, IEnumerable<Crop> crops)
    {
        return dto.Select(d =>
        {
            var crop = cropsNormalization?.Where(crop => crop.ProtocolId == d.ProtocolId).Select(crop => crop.CropId);
            var cropsName = crop?.Select(cropId =>
            {
                var matchedCrop = crops.FirstOrDefault(c => c.CropId == cropId);

                return matchedCrop?.CropName;
            });
            return new ProtocolDtoDisplay()
            {
                ProtocolId = d.ProtocolId,
                CashFlowId = d.CashFlowId,
                ReportId = d.ReportId,
                EntryDate = d.EntryDate,
                ReportDate = d.ReportDate,
                IsCollectedByClient = d.IsCollectedByClient,
                TransactionId = d.TransactionId,
                TotalPaid = d.TotalPaid,
                PaymentDate = d.PaymentDate,
                ClientId = d.ClientId,
                ClientName = d.ClientName,
                ClientTaxId = d.ClientTaxId,
                PropertyId = d.PropertyId,
                StateId = d.StateId,
                StateCode = d.StateCode,
                PropertyName = d.PropertyName,
                City = d.City,
                PostalCode = d.PostalCode,
                Area = d.Area,
                Ccir = d.Ccir,
                ItrNirf = d.ItrNirf,
                PartnerId = d.PartnerId,
                PartnerName = d.PartnerName,
                CatalogId = d.CatalogId,
                Price = d.Price,
                ReportType = d.ReportType,
                Results = d.Results?
                .Select(result => JsonSerializer.Deserialize<ReportResult>(result))
                .ToArray()!,
                Crops = crop?.ToArray(),
            };
        });
    }

    public static Protocol ToProtocol(this ProtocolDtoPatch dto)
    => new Protocol()
    {
        ProtocolId = dto.ProtocolId,
        CashFlowId = default,
        ReportId = default,
        ClientId = default,
        PropertyId = default,
        PartnerId = dto.PartnerId,
        CatalogId = default,
        EntryDate = dto.EntryDate,
        ReportDate = dto.ReportDate,
        IsCollectedByClient = dto.IsCollectedByClient,
    };

    public static Protocol ToProtocol(this ReportDtoReset dto)
    => new Protocol()
    {
        ProtocolId = dto.ProtocolId,
        CashFlowId = default,
        ReportId = default,
        ClientId = default,
        PropertyId = default,
        PartnerId = default,
        CatalogId = dto.CatalogId,
        EntryDate = default,
        ReportDate = default,
        IsCollectedByClient = default,
    };

    // public static Report ToReport(this ProtocolDtoUpdate dto)
    // =>
    //  new Report()
    //  {
    //      ReportId = dto.ReportId,
    //      Results = dto.Results
    //  };
    public static (ReportDtoAdd, Report) ToReport(this ProtocolDtoUpdate dto)
    => (new ReportDtoAdd()
    {
        ProtocolId = dto?.ProtocolId,
        Results = dto?.Results
    },
     new Report()
     {
         ReportId = dto?.ReportId,
         Results = dto?.Results
     });

    public static Protocol ToProtocol(this ProtocolDtoUpdateCashFlow dto)
    => new Protocol()
    {
        ProtocolId = dto.ProtocolId,
        CashFlowId = dto.CashFlowId,
        ReportId = default,
        ClientId = default,
        PropertyId = default,
        PartnerId = default,
        CatalogId = default,
        EntryDate = default,
        ReportDate = default,
        IsCollectedByClient = default,
    };

    public static CashFlow ToCashFlow(this ProtocolDtoUpdateCashFlow dto)
    => new CashFlow()
    {
        CashFlowId = dto.CashFlowId,
        Description = $"{dto.Description}, {(dto.Description!.Contains("protcolo") ? "," : "protocolo:")} {dto.ProtocolId}",
        TransactionId = default,
        PartnerId = default,
        TotalPaid = default,
        PaymentDate = default
    };
}
