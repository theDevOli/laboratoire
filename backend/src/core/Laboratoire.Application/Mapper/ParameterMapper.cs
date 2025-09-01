using Laboratoire.Domain.Entity;
using Laboratoire.Application.DTO;

namespace Laboratoire.Application.Mapper;

public static class ParameterMapper
{
    public static Parameter ToParameter(this ParameterDtoAdd dto)
    => new Parameter()
    {
        ParameterId = default,
        CatalogId = dto.CatalogId,
        ParameterName = dto.ParameterName?.Trim(),
        Unit = dto.Unit?.Trim(),
        InputQuantity = dto.InputQuantity,
        OfficialDoc = dto.OfficialDoc?.Trim(),
        Vmp = dto.Vmp?.Trim(),
        Equation = dto.Equation?.Trim(),
    };

    public static ParameterDtoDisplay ToDisplay(this Parameter dto)
    => new ParameterDtoDisplay()
    {
        ParameterId = dto.ParameterId,
        ParameterName = dto.ParameterName,
        InputQuantity = dto.InputQuantity,
    };
}
