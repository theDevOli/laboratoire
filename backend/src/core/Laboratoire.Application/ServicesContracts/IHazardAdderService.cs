using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IHazardAdderService
{
    Task<Error> AddHazardAsync(HazardDtoAdd hazardDto);
}
