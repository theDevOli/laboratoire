using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class PropertyGetterService
(
    IPropertyRepository propertyRepository,
    ILogger<PropertyGetterService> logger
)
: IPropertyGetterService
{

    public Task<IEnumerable<Property>> GetAllPropertiesAsync()
    {
        logger.LogInformation("Fetching all properties from the repository.");

        return propertyRepository.GetAllPropertiesAsync();
    }
}
