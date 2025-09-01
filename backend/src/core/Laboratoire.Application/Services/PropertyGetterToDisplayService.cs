using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class PropertyGetterToDisplayService
(
    IPropertyRepository propertyRepository,
    ILogger<PropertyGetterToDisplayService> logger
)
: IPropertyGetterToDisplayService
{
    public Task<IEnumerable<PropertyDtoDisplay>> GetAllPropertiesDisplayAsync()
    {
        logger.LogInformation("Fetching all properties to display.");

        return propertyRepository.GetAllPropertiesDisplayAsync<PropertyDtoDisplay>();
    }
}
