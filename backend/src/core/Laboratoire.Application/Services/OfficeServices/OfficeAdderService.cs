using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.OfficeServices;

public class OfficeAdderService
(
    IOfficeRepository officeRepository,
    ILogger<OfficeAdderService> logger
)
: IOfficeAdderService
{
    public async Task<Error> AddOfficeAsync(Office office)
    {
        logger.LogInformation("Adding a new office");
        var exists = await officeRepository.DoesOfficeExistByCityAndNameAsync(office);

        if (exists)
        {
            logger.LogError("The resources to be added is already within the database!");
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        var ok = await officeRepository.AddOfficeAsync(office);
        if (!ok)
        {
            logger.LogError("Database error operation to add new office!");
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        return Error.SetSuccess();
    }
}
