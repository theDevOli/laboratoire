using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.CropServices;

public class CropsNormalizationGetterByReportIdService
(
    ICropsNormalizationRepository cropsNormalizationRepository,
    ILogger<CropsNormalizationGetterByReportIdService> logger
)
: ICropsNormalizationGetterByReportIdService
{
    public async Task<IEnumerable<CropsNormalization>?> GetCropByReportIdAsync(Guid? reportId)
    {
        if (reportId is null)
        {
            logger.LogWarning("GetCropByReportIdAsync was called with null reportId.");
            return null;
        }

        logger.LogInformation("Fetching crop normalizations for report ID: {ReportId}", reportId);
        return await cropsNormalizationRepository.GetCropByReportIdAsync(reportId);
    }
}
