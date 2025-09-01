using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICropsNormalizationGetterByReportIdService
{
    Task<IEnumerable<CropsNormalization>?> GetCropByReportIdAsync(Guid? reportId);
}
