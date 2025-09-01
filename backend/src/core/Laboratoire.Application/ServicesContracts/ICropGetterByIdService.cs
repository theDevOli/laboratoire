using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface ICropGetterByIdService
{
    Task<Crop?> GetCropByIdAsync(int? cropId);
}
