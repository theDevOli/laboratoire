
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ProtocolServices;

public class ProtocolPatchReportService
(
    IProtocolRepository protocolRepository,
    ILogger<ProtocolPatchReportService> logger
)
: IProtocolPatchReportService
{
    public async Task<Error> PatchReportIdAsync(ReportPatch reportPatch)
    {
        logger.LogInformation("Starting report ID patch for report ID: {ReportId}", reportPatch.ReportId);

        var exists = await protocolRepository.DoesProtocolExistByProtocolIdAsync(reportPatch.ProtocolId);
        if (!exists)
        {
            logger.LogWarning("Protocol with ID {ProtocolId} not found.", reportPatch.ProtocolId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var isUpdated = await protocolRepository.PatchReportIdAsync(reportPatch);
        if (!isUpdated)
        {
            logger.LogError("Failed to patch report ID for protocol ID: {ProtocolId}", reportPatch.ReportId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Report ID patched successfully for protocol ID: {ProtocolId}", reportPatch.ReportId);
        return Error.SetSuccess();
    }
}
