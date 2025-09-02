using System;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Entity;
using Laboratoire.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;

namespace Laboratoire.Application.Services.CashFlowServices;

public class CashFlowGetterByYearAndMonthService
(
    ICashFlowRepository cashFlowRepository,
    ILogger<CashFlowGetterByYearAndMonthService> logger
)
: ICashFlowGetterByYearAndMonthService
{
    public async Task<IEnumerable<CashFlow>> GetCashFlowByYearAndMonthAsync(int? year, int? month)
    {
        if (year is null || month is null)
        {
            logger.LogWarning("GetCashFlowByYearAndMonthAsync called with null year or month. Year: {Year}, Month: {Month}", year, month);
            return Enumerable.Empty<CashFlow>();
        }

        logger.LogInformation("Retrieving cash flows for Year: {Year}, Month: {Month}", year, month);
        return await cashFlowRepository.GetCashFlowByYearAndMonthAsync(year, month);
    }
}
