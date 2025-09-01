using System.Text.Json;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class CatalogMapper
{
    public static Catalog ToCatalog(this CatalogDtoAdd dto)
    => new Catalog()
    {
        CatalogId = default,
        ReportType = dto.ReportType?.Trim(),
        SampleType = dto.SampleType?.Trim(),
        LabelName = dto.LabelName?.Trim(),
        Legends = dto.Legends,
        Price = dto.Price,
    };
    public static Catalog FromDb(this CatalogDtoDb dto)
    => new Catalog()
    {
        CatalogId = dto.CatalogId,
        ReportType = dto.ReportType,
        SampleType = dto.SampleType,
        LabelName = dto.LabelName,
        Legends = dto?.Legends?
        .Select(legend => JsonSerializer.Deserialize<Legend>(legend))
        .ToArray()!,
        Price = dto?.Price,
    };

    public static CatalogDtoDb ToDb(this Catalog catalog)
    => new CatalogDtoDb()
    {
        CatalogId = catalog.CatalogId,
        ReportType = catalog.ReportType,
        SampleType = catalog.SampleType,
        LabelName = catalog.LabelName,
        Legends = catalog?.Legends?
        .Select(legend => JsonSerializer.Serialize(legend))
        .ToArray()!,
        Price = catalog?.Price,
    };

}
