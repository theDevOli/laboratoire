using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services;

public class ReportPatchService
(
    IReportRepository reportRepository,
    ILogger<ReportPatchService> logger
)
: IReportPatchService
{
    public async Task<Error> PatchReportAsync(Report report)
    {
        logger.LogInformation("Starting patch process for report ID: {ReportId}", report.ReportId);

        var exist = await reportRepository.DoesReportExistsAsync(report);
        if (!exist)
        {
            logger.LogWarning("Report with ID {ReportId} not found.", report.ReportId);
            return Error.SetError(ErrorMessage.NotFound, 404);
        }

        var ok = await reportRepository.PatchReportAsync(report);
        if (!ok)
        {
            logger.LogError("Failed to patch report with ID {ReportId}.", report.ReportId);
            return Error.SetError(ErrorMessage.DbError, 500);
        }

        logger.LogInformation("Report with ID {ReportId} patched successfully.", report.ReportId);
        return Error.SetSuccess();
    }
}
