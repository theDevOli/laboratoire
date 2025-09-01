
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ProtocolPatchReportService
(
    IProtocolRepository protocolRepository,
    ILogger<ProtocolPatchReportService> logger
)
: IProtocolPatchReportService
{
    public async Task<Error> PatchReportIdAsync(Report report)
    {
        logger.LogInformation("Starting report ID patch for protocol ID: {ProtocolId}", report.ProtocolId);

        var exists = await protocolRepository.DoesProtocolExistByProtocolIdAsync(report.ProtocolId);
        if (!exists)
        {
            logger.LogWarning("Protocol with ID {ProtocolId} not found.", report.ProtocolId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var isUpdated = await protocolRepository.PatchReportIdAsync(report);
        if (!isUpdated)
        {
            logger.LogError("Failed to patch report ID for protocol ID: {ProtocolId}", report.ProtocolId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Report ID patched successfully for protocol ID: {ProtocolId}", report.ProtocolId);
        return Error.SetSuccess();
    }
}
