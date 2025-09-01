using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPropertyGetterByClientIdService
{
    Task<IEnumerable<Property>?> GetAllPropertiesByClientIdAsync(Guid? clientId);
}
