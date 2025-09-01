using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Domain.Utils;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
[Authorize(Policy =Policy.Workers)]
public class TransactionController
(
    ITransactionGetterService transactionGetterService,
    ITransactionGetterByIdService transactionGetterByIdService,
    ITransactionAdderService transactionAdderService,
    ITransactionUpdatableService transactionUpdatableService
)
: ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllTransactionsAsync()
    {
        var transactions = await transactionGetterService.GetAllTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<Transaction>>.Success(transactions));
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetAllTransactionsAsync([FromRoute] int transactionId)
    {
        var transaction = await transactionGetterByIdService.GetTransactionByIdAsync(transactionId);
        if (transaction is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Transaction>.Success(transaction));
    }

    [HttpPost]
    public async Task<IActionResult> AddTransactionAsync([FromBody] TransactionDtoAdd transactionDto)
    {
        var addError = await transactionAdderService.AddTransactionAsync(transactionDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{transactionId}")]
    public async Task<IActionResult> UpdateTransactionAsync([FromRoute] int transactionId, [FromBody] Transaction transaction)
    {
        if (transactionId != transaction.TransactionId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await transactionUpdatableService.UpdateTransactionAsync(transaction);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}