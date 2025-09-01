using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface IHazardUpdatableService
{
    Task<Error> UpdateHazardAsync(Hazard hazard);
}
