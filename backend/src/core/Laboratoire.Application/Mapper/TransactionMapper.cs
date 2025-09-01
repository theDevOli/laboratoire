
using Laboratoire.Application.DTO;
using Laboratoire.Domain.Entity;

namespace Laboratoire.Application.Mapper;

public static class TransactionMapper
{
    public static Transaction ToTransaction(this TransactionDtoAdd dto)
    => new Transaction()
    {
        TransactionId = default,
        TransactionType = dto.TransactionType?.Trim(),
        OwnerName = dto.OwnerName?.Trim(),
        BankName = dto.BankName?.Trim(),
    };
}

