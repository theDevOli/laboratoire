using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;

namespace Laboratoire.Application.ServicesContracts;

public interface IReportAdderService
{
    Task<Error> AddReportAsync(ReportDtoAdd report);
}
