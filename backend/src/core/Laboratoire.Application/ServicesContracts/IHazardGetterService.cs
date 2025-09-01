using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IHazardGetterService
{
    Task<IEnumerable<Hazard>> GetAllHazardsAsync();
}
