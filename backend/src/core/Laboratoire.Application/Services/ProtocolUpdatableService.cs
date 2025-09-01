using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;
using Laboratoire.Application.Mapper;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ProtocolUpdatableService
(
    IProtocolRepository protocolRepository,
    IProtocolPatchCatalogService protocolPatchCatalogService,
    IReportAdderService reportAdderService,
    IReportPatchService reportPatchService,
    ICashFlowUpdatableService cashFlowUpdatableService,
    ICropsNormalizationAdderService cropsNormalizationAdderService,
    ICashFlowAdderService cashFlowAdderService,
    ILogger<ProtocolUpdatableService> logger
)
: IProtocolUpdatableService
{
    public async Task<Error> UpdateProtocolAsync(ProtocolDtoUpdate protocolDto)

    {
        logger.LogInformation("Starting update process for protocol ID: {ProtocolId}", protocolDto.ProtocolId);

        var protocol = protocolDto.ToProtocol();

        var tasks = new List<Task<Error>>();

        var protocolDb = await protocolRepository.GetProtocolByProtocolIdAsync(protocol.ProtocolId);
        if (protocolDb is null)
        {
            logger.LogWarning("Protocol with ID {ProtocolId} not found.", protocol.ProtocolId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var toResetResults = protocolDb.IsNotSameCatalog(protocol);

        if (toResetResults)
        {
            logger.LogInformation("Catalog has changed. Scheduling catalog update.");
            tasks.Add(protocolPatchCatalogService.UpdateCatalogAsync(protocol));
        }

        var report = protocolDto.ToReport();
        if (report.ReportId is null && !toResetResults)
        {
            logger.LogInformation("No Report ID found and catalog not reset. Scheduling report creation.");
            tasks.Add(reportAdderService.AddReportAsync(report));
        }

        if (report.ReportId is not null && !toResetResults)
        {
            logger.LogInformation("Report ID found and catalog not reset. Scheduling report patch.");
            tasks.Add(reportPatchService.PatchReportAsync(report));
        }

        var ok = await protocolRepository.UpdateProtocolAsync(protocol);
        if (!ok)
        {
            logger.LogError("Failed to update protocol with ID {ProtocolId}.", protocol.ProtocolId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }
        logger.LogInformation("Protocol with ID {ProtocolId} updated successfully.", protocol.ProtocolId);

        var cashFlow = protocolDto.ToCashFlow();
        if (protocolDb.ToAddCashFlow() && cashFlow.TotalPaid is not null)
        {
            logger.LogInformation("CashFlow needs to be added. Scheduling cash flow creation.");
            tasks.Add(cashFlowAdderService.AddCashFlowAsync(cashFlow, protocol));
        }

        if (!protocolDb.ToAddCashFlow()
        && cashFlow.TotalPaid is not null
        && cashFlow.CashFlowId is not null)
        {
            logger.LogInformation("Existing CashFlow found. Scheduling cash flow update.");
            tasks.Add(cashFlowUpdatableService.UpdateCashFlowAsync(cashFlow));
        }

        var cropsNormalization = protocolDto.ToCropsNormalization();
        logger.LogInformation("Scheduling crops normalization addition.");
        tasks.Add(cropsNormalizationAdderService.AddCropsAsync(cropsNormalization, protocol.ProtocolId!));

        logger.LogInformation("Executing {TaskCount} asynchronous operations.", tasks.Count);
        var errors = await Task.WhenAll(tasks);
        foreach (var error in errors)
        {
            if (error.IsNotSuccess())
            {
                logger.LogError("An operation failed during protocol update. Error: {Error}", error.Message);
                return error;
            }
        }

        logger.LogInformation("Protocol update process completed successfully for ID: {ProtocolId}", protocol.ProtocolId);
        return Error.SetSuccess();
    }
}
