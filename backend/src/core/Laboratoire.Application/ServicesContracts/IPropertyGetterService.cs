using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IPropertyGetterService
{
    Task<IEnumerable<Property>> GetAllPropertiesAsync();
}
