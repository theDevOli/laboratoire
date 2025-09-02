using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.ServicesContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.ReportServices;

public class ReportGetterService
(
    IReportRepository reportRepository,
    ILogger<ReportGetterService> logger
)
: IReportGetterService
{
    public Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        logger.LogInformation("Fetching all reports.");

        return reportRepository.GetAllReportsAsync();
    }
}
