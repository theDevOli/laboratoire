using Laboratoire.Domain.Entity;

namespace Laboratoire.Domain.RepositoryContracts;

public interface ICropRepository
{
    Task<IEnumerable<Crop>> GetAllCropsAsync();
    Task<Crop?> GetCropByIdAsync(int? cropId);
    Task<Crop?> GetCropByNameAsync(string? cropName);
    Task<bool> AddCropAsync(Crop crop);
    Task<bool> DoesCropExistByCropIdAsync(Crop crop);
    Task<bool> DoesCropExistByNameAsync(Crop crop);
    Task<bool> UpdateCropAsync(Crop crop);
}
