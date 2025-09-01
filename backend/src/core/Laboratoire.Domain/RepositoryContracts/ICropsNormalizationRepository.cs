using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface ICropsNormalizationRepository
{
    Task<IEnumerable<CropsNormalization>> GetAllCropsAsync();
    Task<IEnumerable<CropsNormalization>?> GetCropByReportIdAsync(Guid? reportId);
    Task<bool> AddCropsAsync(IEnumerable<CropsNormalization> cropsNormalization);
    Task<bool> IsThereNoneCropsAsync(string? protocolId);
    Task<bool> DeleteCropsAsync(string? protocolId);
}
