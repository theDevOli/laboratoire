using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IPropertyGetterToDisplayService
{
    Task<IEnumerable<PropertyDtoDisplay>> GetAllPropertiesDisplayAsync();
}
