using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface ICropUpdatableService
{
    Task<Error> UpdateCropAsync(Crop crop);
}
