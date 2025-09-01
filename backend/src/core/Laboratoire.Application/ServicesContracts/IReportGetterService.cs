using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.ServicesContracts;

public interface IReportGetterService
{
    Task<IEnumerable<Report>> GetAllReportsAsync();

}
