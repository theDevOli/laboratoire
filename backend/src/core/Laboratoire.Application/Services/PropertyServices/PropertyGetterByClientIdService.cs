using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.PropertyServices;

public class PropertyGetterByClientIdService
(
    IPropertyRepository propertyRepository,
    ILogger<PropertyGetterByClientIdService> logger
)
: IPropertyGetterByClientIdService
{
    public async Task<IEnumerable<Property>?> GetAllPropertiesByClientIdAsync(Guid? clientId)
    {
        if (clientId is null)
        {
            logger.LogWarning("GetAllPropertiesByClientIdAsync called with null clientId.");
            return null;
        }

        logger.LogInformation("Retrieving properties for clientId: {ClientId}", clientId);

        return await propertyRepository.GetAllPropertiesByClientIdAsync(clientId);
    }
}
