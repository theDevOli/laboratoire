using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class CashFlowMapper
{
    public static CashFlow ToCashFlow(this CashFlowDtoAdd dto)
    => new CashFlow()
    {
        CashFlowId = default,
        Description = dto.Description?.Trim(),
        TransactionId = dto.TransactionId,
        PartnerId = dto.PartnerId,
        TotalPaid = dto.TotalPaid,
        PaymentDate = dto.PaymentDate,
    };

}
