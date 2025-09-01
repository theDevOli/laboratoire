using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IHazardGetterByIdService
{
    Task<Hazard?> GetHazardByIdAsync(int? hazardId);
}
