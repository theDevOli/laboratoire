using System.Text.Json;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class CropMapper
{
    public static Crop ToCrop(this CropDtoAdd dto)
    => new Crop()
    {
        CropId = default,
        CropName = dto.CropName?.Trim(),
        NitrogenCover = dto.NitrogenCover,
        NitrogenFoundation = dto.NitrogenFoundation,
        Phosphorus = dto.Phosphorus,
        Potassium = dto.Potassium,
        MinV = dto.MinV,
        ExtraData = dto.ExtraData?.Trim(),
    };
    public static IEnumerable<CropDtoAdd>? ToCropDtoAdd(this IEnumerable<Crop> dto, IEnumerable<int?>? cropsId)
    => cropsId?
    .Select(cropId =>
        {
            var crop = dto?.FirstOrDefault(firstCrop => firstCrop.CropId.Equals(cropId));
            return new CropDtoAdd()
            {
                CropName = crop?.CropName,
                NitrogenCover = crop?.NitrogenCover,
                NitrogenFoundation = crop?.NitrogenFoundation,
                Phosphorus = crop?.Phosphorus,
                Potassium = crop?.Potassium,
                MinV = crop?.MinV,
                ExtraData = crop?.ExtraData,
            };

        });
    public static Crop FromDb(this CropDtoDb dto)
    => new Crop()
    {
        CropId = dto.CropId,
        CropName = dto.CropName,
        NitrogenCover = dto.NitrogenCover,
        NitrogenFoundation = dto.NitrogenFoundation,
        Phosphorus = JsonSerializer.Deserialize<CropParameter>(dto.Phosphorus!),
        Potassium = JsonSerializer.Deserialize<CropParameter>(dto.Potassium!),
        MinV = dto.MinV,
        ExtraData = dto.ExtraData,
    };

    public static CropDtoDb ToDb(this Crop crop)
    => new CropDtoDb()
    {
        CropId = crop.CropId,
        CropName = crop.CropName,
        NitrogenCover = crop.NitrogenCover,
        NitrogenFoundation = crop.NitrogenFoundation,
        Phosphorus = JsonSerializer.Serialize<CropParameter>(crop.Phosphorus!),
        Potassium = JsonSerializer.Serialize<CropParameter>(crop.Potassium!),
        MinV = crop.MinV,
        ExtraData = crop.ExtraData
    };
}
