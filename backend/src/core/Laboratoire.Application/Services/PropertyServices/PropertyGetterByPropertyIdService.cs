using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.PropertyServices;

public class PropertyGetterByPropertyIdService
(
    IPropertyRepository propertyRepository,
    ILogger<PropertyGetterByPropertyIdService> logger
)
: IPropertyGetterByPropertyIdService
{

    public async Task<Property?> GetPropertyByPropertyIdAsync(int? propertyId)
    {
        if (propertyId is null)
        {
            logger.LogWarning("GetPropertyByPropertyIdAsync was called with null propertyId.");
            return null;
        }

        logger.LogInformation("Retrieving property with ID: {PropertyId}", propertyId);

        return await propertyRepository.GetPropertyByIdAsync(propertyId);
    }
}
