using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.PropertyServices;

public class PropertyAdderService
(
    IPropertyRepository propertyRepository,
    IUtilsRepository utilsRepository,
    ILogger<PropertyAdderService> logger
)
: IPropertyAdderService
{
    public async Task<Error> AddPropertyAsync(PropertyDtoAdd propertyDto)
    {
        logger.LogInformation("Starting to add a new property for City: {City}, StateId: {StateId}", propertyDto.City, propertyDto.StateId);

        var property = propertyDto.ToProperty();

        if (property.PostalCode is null)
        {
            logger.LogInformation("Postal code not provided. Attempting to retrieve using city and state.");
            var postalCode = await utilsRepository.GetPostalCodeByCityAndStateAsync(property.City, property.StateId);
            property.PostalCode = postalCode;
            logger.LogInformation("Retrieved postal code: {PostalCode}", postalCode);
        }

        var ok = await propertyRepository.AddPropertyAsync(property);
        if (!ok)
        {
            logger.LogError("Failed to add property for City: {City}, StateId: {StateId}", property.City, property.StateId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Successfully added property for City: {City}, StateId: {StateId}", property.City, property.StateId);
        return Error.SetSuccess();
    }
}
