using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class FertilizerMapper
{
    public static FertilizerDtoGet ToDto(this Fertilizer dto)
    => new FertilizerDtoGet()
    {
        FertilizerId = dto.FertilizerId,
        Proportion = dto.Proportion,
        Formulation = $"{dto.Nitrogen}-{dto.Phosphorus}-{dto.Potassium}"
    };
    public static IEnumerable<FertilizerDtoGet> ToDto(this IEnumerable<Fertilizer> dto)
    => dto.Select(d => new FertilizerDtoGet()
    {
        FertilizerId = d.FertilizerId,
        Proportion = d.Proportion,
        Formulation = $"{d.Nitrogen}-{d.Phosphorus}-{d.Potassium}"
    });

}
