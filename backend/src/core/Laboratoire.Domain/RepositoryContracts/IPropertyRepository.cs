using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface IPropertyRepository
{
    Task<IEnumerable<Property>> GetAllPropertiesAsync();
    Task<IEnumerable<Property>> GetAllPropertiesByClientIdAsync(Guid? clientId);
    Task<IEnumerable<T>> GetAllPropertiesDisplayAsync<T>();
    Task<Property?> GetPropertyByIdAsync(int? propertyId);
    Task<bool> DoesPropertyExistAsync(Property property);
    Task<bool> AddPropertyAsync(Property property);
    Task<bool> UpdatePropertyAsync(Property property);
}
