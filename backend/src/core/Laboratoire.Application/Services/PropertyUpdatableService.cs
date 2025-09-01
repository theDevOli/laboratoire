using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class PropertyUpdatableService
(
    IPropertyRepository propertyRepository,
    ILogger<PropertyUpdatableService> logger
)
: IPropertyUpdatableService
{
    public async Task<Error> UpdatePropertyAsync(Property property)
    {
        logger.LogInformation("Attempting to update property with ID {PropertyId}.", property.PropertyId);

        // var exists = await propertyRepository.DoesPropertyExistAsync(property);
        var propertyDb = await propertyRepository.GetPropertyByIdAsync(property.PropertyId);
        if (propertyDb is null)
        {
            logger.LogWarning("Property with ID {PropertyId} not found.", property.PropertyId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await propertyRepository.UpdatePropertyAsync(property);
        if (!ok)
        {
            logger.LogError("Database error while updating property with ID {PropertyId}.", property.PropertyId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Successfully updated property with ID {PropertyId}.", property.PropertyId);
        return Error.SetSuccess();
    }
}
