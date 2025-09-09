using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.ServicesContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ReportServices;

public class ReportGetterByIdService
(
    IReportRepository reportRepository,
    ILogger<ReportGetterByIdService> logger
)
: IReportGetterByIdService
{
    public Task<Report?> GetReportByIdAsync(Guid? reportId)
    {
        logger.LogInformation("Fetching report by ID: {ReportId}", reportId);

        if (reportId is null)
        {
            logger.LogWarning("GetReportByIdAsync called with null reportId.");
            return Task.FromResult<Report?>(null);
        }

        return reportRepository.GetReportByIdAsync(reportId);
    }
}
