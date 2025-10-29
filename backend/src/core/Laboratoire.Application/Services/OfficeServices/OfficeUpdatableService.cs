using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.OfficeServices;

public class OfficeUpdatableService
(
    IOfficeRepository officeRepository,
    ILogger<OfficeUpdatableService> logger
)
: IOfficeUpdatableService
{
    public async Task<Error> UpdateOfficeAsync(Office office)
    {
        logger.LogInformation("Starting office update process with ID: {}.", office.OfficeId);

        var exists = await officeRepository.DoesOfficeExistByIdAsync(office);
        if (!exists)
        {
            logger.LogError("The office with ID:{}, was not found on the database.", office.OfficeId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await officeRepository.UpdateOfficeAsync(office);
        if (!ok)
        {
            logger.LogError("Database error while updating office with ID: {}.", office.OfficeId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }
        
        return Error.SetSuccess();
    }
}
