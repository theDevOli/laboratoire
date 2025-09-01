using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ReportAdderService
(
    IReportRepository reportRepository,
    IProtocolPatchReportService protocolPatchReportService,
    IParameterGetterService parameterGetterService,
    ILogger<ReportAdderService> logger
)
: IReportAdderService
{
    public async Task<Error> AddReportAsync(Report report)
    {
        logger.LogInformation("Starting to add report for protocol ID: {ProtocolId}", report.ProtocolId);
        var isDoubled = await reportRepository.IsProtocolDoubled(report);
        if (isDoubled)
        {
            logger.LogWarning("A report already exists for protocol ID: {ProtocolId}. Conflict detected.", report.ProtocolId);
            return Error.SetError(ErrorMessage.ConflictPost, 409);
        }

        logger.LogInformation("Fetching parameters to generate report equations.");
        var parameters = await parameterGetterService.GetAllParametersAsync();
        report.AddEquations(parameters);
        logger.LogDebug("Equations added to report: {@Report}", report);

        var reportId = await reportRepository.AddReportAsync(report);
        if (reportId is null)
            if (reportId is null)
            {
                logger.LogError("Failed to insert report into the database for protocol ID: {ProtocolId}", report.ProtocolId);
                return Error.SetError("The report was not found in the database", 404);
            }

        report.ReportId = reportId;
        logger.LogInformation("Report added successfully with ID: {ReportId}. Proceeding to patch protocol.", reportId);

        return await protocolPatchReportService.PatchReportIdAsync(report);
    }
}
