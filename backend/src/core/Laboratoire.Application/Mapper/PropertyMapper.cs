using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class PropertyMapper
{
    public static Property ToProperty(this PropertyDtoAdd dto)
    => new Property()
    {
        PropertyId = default,
        ClientId = dto.ClientId,
        StateId = dto.StateId,
        PropertyName = dto.PropertyName?.Trim(),
        Registration = dto.Registration?.Trim(),
        City = dto.City?.Trim(),
        PostalCode = dto.PostalCode?.Trim(),
        Area = dto.Area?.Trim(),
        Ccir = dto.Ccir?.Trim(),
        ItrNirf = dto.ItrNirf?.Trim()
    };
}
