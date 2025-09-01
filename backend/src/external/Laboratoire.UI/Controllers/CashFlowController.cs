using Microsoft.AspNetCore.Mvc;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Laboratoire.Application.Mapper;
using Laboratoire.Domain.Utils;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class CashFlowController
(
    ICashFlowGetterService cashFlowGetterService,
    ICashFlowGetterByIdService cashFlowGetterByIdService,
    ICashFlowGetterByYearAndMonthService cashFlowGetterByYearAndMonthService,
    ICashFlowUpdatableService cashFlowUpdatableService,
    ICashFlowAdderService cashFlowAdderService
)
: ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policy.Workers)]
    public async Task<IActionResult> GetAllCashFlowAsync()
    {
        var cashFlow = await cashFlowGetterService.GetAllCashFlowAsync();
        return Ok(ApiResponse<IEnumerable<CashFlow>>.Success(cashFlow));
    }

    [HttpGet("{cashFlowId}")]
    [Authorize(Policy = Policy.Workers)]
    public async Task<IActionResult> GetCashFlowByIdAsync([FromRoute] int cashFlowId)
    {
        var cashFlow = await cashFlowGetterByIdService.GetCashFlowByIdAsync(cashFlowId);
        if (cashFlow is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<CashFlow>.Success(cashFlow));
    }

    [HttpGet("{year}/{month}")]
    [Authorize(Policy = Policy.Workers)]
    public async Task<IActionResult> GetCashFlowByYearAndMonthAsync
    (
        [FromRoute] int year,
        [FromRoute] int month,
        [FromQuery] string cashFlowFilter,
        [FromQuery] int transactionFilter,
        [FromQuery] Guid? partnerId
    )
    {
        var cashFlow = await cashFlowGetterByYearAndMonthService.GetCashFlowByYearAndMonthAsync(year, month);

        if (transactionFilter > 1)
            cashFlow = cashFlow.Where(p => p.TransactionId == transactionFilter);

        if (cashFlowFilter == "in")
            cashFlow = cashFlow.Where(p => p.TotalPaid > 0);

        if (cashFlowFilter == "out")
            cashFlow = cashFlow.Where(p => p.TotalPaid < 0);

        if (partnerId is not null)
            cashFlow = cashFlow.Where(p => p.PartnerId == partnerId);

        var totalAmount = cashFlow.Sum(p => p.TotalPaid);

        var cashFlowDisplay = new CashFlowDtoDisplay()
        {
            CashFlow = cashFlow,
            TotalAmount = totalAmount,
        };

        return Ok(ApiResponse<CashFlowDtoDisplay>.Success(cashFlowDisplay));
    }

    [HttpPut("{cashFlowId}")]
    [Authorize(Roles = "admin,recepção")]
    public async Task<IActionResult> UpdateCashFlowAsync([FromRoute] int cashFlowId, [FromBody] CashFlow cashFlow)
    {
        if (cashFlowId != cashFlow.CashFlowId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await cashFlowUpdatableService.UpdateCashFlowAsync(cashFlow);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }

    [HttpPost]
    [Authorize(Roles = "admin,recepção")]
    public async Task<IActionResult> AddCashFlowAsync(CashFlowDtoAdd dto)
    {
        var cashFlow = dto.ToCashFlow();
        var error = await cashFlowAdderService.AddCashFlowAsync(cashFlow, null);
        if (error.IsNotSuccess())
            return StatusCode(error.StatusCode, ApiResponse<object>.Failure(error!.Message!, error!.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }
}