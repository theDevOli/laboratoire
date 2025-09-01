using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IFertilizerGetterService
{
    Task<IEnumerable<FertilizerDtoGet>> GetAllFertilizersAsync();
}
