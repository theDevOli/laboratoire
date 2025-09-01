using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IHazardRepository
{
    Task<IEnumerable<Hazard>> GetAllHazardsAsync();
    Task<Hazard?> GetHazardByIdAsync(int? hazardId);
    Task<Hazard?> GetHazardByClassAsync(string? hazardClass);
    Task<bool> DoesHazardExistByIdAsync(Hazard hazard);
    Task<bool> DoesHazardExistByClassAsync(Hazard hazard);
    Task<bool> AddHazardAsync(Hazard hazard);
    Task<bool> UpdateHazardAsync(Hazard hazard);

}
