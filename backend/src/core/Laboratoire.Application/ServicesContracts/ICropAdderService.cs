using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;

namespace Laboratoire.Application.ServicesContracts;

public interface ICropAdderService
{
    Task<Error> AddCropAsync(CropDtoAdd cropDto);
}
