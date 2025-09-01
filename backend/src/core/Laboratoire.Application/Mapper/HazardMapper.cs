using System.Text.Json;
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class HazardMapper
{
    public static Hazard ToHazard(this HazardDtoAdd dto)
    => new Hazard()
    {
        HazardId = default,
        HazardClass = dto.HazardClass,
        HazardName = dto.HazardName
    };
}
