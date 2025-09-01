

using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class ChemicalMapper
{
    public static Chemical ToChemical(this ChemicalDtoAdd dto)
    => new Chemical()
    {
        ChemicalId = default,
        ChemicalName = dto.ChemicalName,
        Concentration = dto.Concentration,
        Quantity = dto.Quantity,
        Unit = dto.Unit,
        IsPoliceControlled = dto.IsPoliceControlled,
        IsArmyControlled = dto.IsArmyControlled,
        EntryDate = dto.EntryDate,
        ExpireDate = dto.ExpireDate,
    };
    public static Chemical ToChemical(this ChemicalDtoGetUpdate dto)
        => new Chemical()
        {
            ChemicalId = dto.ChemicalId,
            ChemicalName = dto.ChemicalName,
            Concentration = dto.Concentration,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            IsPoliceControlled = dto.IsPoliceControlled,
            IsArmyControlled = dto.IsArmyControlled,
            EntryDate = dto.EntryDate,
            ExpireDate = dto.ExpireDate,
        };
    public static ChemicalDtoGetUpdate ToChemicalNormalization(this Chemical dto, IEnumerable<ChemicalsNormalization> hazards)
    {
        var tempHazards = hazards
        .Where(hazard => hazard.ChemicalId == dto.ChemicalId)
        .Select(hazard => hazard.HazardId)
        .ToArray();
        return new ChemicalDtoGetUpdate()
        {
            ChemicalId = dto.ChemicalId,
            ChemicalName = dto.ChemicalName,
            Concentration = dto.Concentration,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            IsPoliceControlled = dto.IsPoliceControlled,
            IsArmyControlled = dto.IsArmyControlled,
            EntryDate = dto.EntryDate,
            ExpireDate = dto.ExpireDate,
            Hazards = tempHazards,
        };
    }
}
