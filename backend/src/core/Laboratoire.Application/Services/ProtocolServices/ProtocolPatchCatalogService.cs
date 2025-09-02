using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ProtocolServices;

public class ProtocolPatchCatalogService
(
    IProtocolRepository protocolRepository,
    IReportRepository reportRepository,
    ILogger<ProtocolPatchCatalogService> logger
)
: IProtocolPatchCatalogService
{
    public async Task<Error> UpdateCatalogAsync(Protocol protocol)
    {
        logger.LogInformation("Starting catalog update for protocol ID: {ProtocolId}", protocol.ProtocolId);
        var ok = await protocolRepository.UpdateCatalogAsync(protocol);
        if (!ok)
        {
            logger.LogError("Failed to update catalog for protocol ID: {ProtocolId}", protocol.ProtocolId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Catalog updated successfully for protocol ID: {ProtocolId}", protocol.ProtocolId);

        var protocolDb = await protocolRepository.GetProtocolByProtocolIdAsync(protocol.ProtocolId);
        var reportId = protocolDb?.ReportId;

        if (reportId is null)
        {
            logger.LogInformation("No report associated with protocol ID: {ProtocolId}. No reset needed.", protocol.ProtocolId);
            return Error.SetSuccess();
        }

        logger.LogInformation("Resetting report ID: {ReportId} associated with protocol ID: {ProtocolId}", reportId, protocol.ProtocolId);

        var isReset = await reportRepository.ResetReportAsync(reportId);
        if (!isReset)
        {
            logger.LogError("Failed to reset report ID: {ReportId} for protocol ID: {ProtocolId}", reportId, protocol.ProtocolId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Report ID: {ReportId} reset successfully for protocol ID: {ProtocolId}", reportId, protocol.ProtocolId);

        logger.LogInformation("Catalog update process completed successfully for protocol ID: {ProtocolId}", protocol.ProtocolId);
        return Error.SetSuccess();
    }
}
