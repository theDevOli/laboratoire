using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Application.ServicesContracts;
using Microsoft.Extensions.Logging;



namespace Laboratoire.Application.Services;

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
        
        return reportRepository.GetReportByIdAsync(reportId);
    }
}
