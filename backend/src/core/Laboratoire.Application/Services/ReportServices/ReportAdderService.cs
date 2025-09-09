using Laboratoire.Domain.RepositoryContracts;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Microsoft.Extensions.Logging;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Mapper;

namespace Laboratoire.Application.Services.ReportServices;

public class ReportAdderService
(
    IReportRepository reportRepository,
    IProtocolPatchReportService protocolPatchReportService,
    IParameterGetterService parameterGetterService,
    ILogger<ReportAdderService> logger
)
: IReportAdderService
{
    public async Task<Error> AddReportAsync(ReportDtoAdd reportDto)
    {

        logger.LogInformation("Fetching parameters to generate report equations.");

        var parameters = await parameterGetterService.GetAllParametersAsync();
        var report = reportDto.ToReport();
        report.AddEquations(parameters);

        logger.LogDebug("Equations added to report: {@Report}", report);
        //TODO:Create a transaction
        var reportId = await reportRepository.AddReportAsync(report);
        if (reportId is null)
        {
            logger.LogError("Failed to insert report into the database for report");
            return Error.SetError(ErrorMessage.DbError, 404);
        }

        var reportDtoPatch = reportDto.ToReportPatch(reportId);
        logger.LogInformation("Report added successfully with ID: {ReportId}. Proceeding to patch protocol.", reportId);

        return await protocolPatchReportService.PatchReportIdAsync(reportDtoPatch);
    }
}
