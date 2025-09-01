using Microsoft.AspNetCore.Mvc;

using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Laboratoire.Domain.Utils;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class ProtocolController
(
    IProtocolAdderService protocolAdderService,
    IProtocolGetterToDisplayService protocolGetterToDisplayService,
    IProtocolYearGetterService protocolYearGetterService,
    IProtocolUpdatableService protocolUpdatableService,
    IProtocolPatchCashFlowIdService protocolPatchCashFlowIdService,
    IProtocolSpotSaverService protocolSpotSaverService
)
: ControllerBase
{
    [HttpGet("DisplayProtocol/{year}")]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> GetDisplayProtocolsAsync
    ([FromRoute] int year)
    {
        var protocol = await protocolGetterToDisplayService.GetDisplayProtocolsAsync(year);
        if (protocol is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<IEnumerable<ProtocolDtoDisplay>>.Success(protocol));
    }

    [HttpGet("DisplayProtocol/{year}/{id}/{isPartner}")]
    [Authorize(Policy =Policy.All)]
    public async Task<IActionResult> GetDisplayProtocolsAsync([FromRoute] int year, [FromRoute] Guid id, [FromRoute] bool isPartner)
    {
        var protocol = await protocolGetterToDisplayService.GetDisplayProtocolsAsync(year, id, isPartner);
        if (protocol is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<IEnumerable<ProtocolDtoDisplay>>.Success(protocol));
    }

    [HttpGet("Years")]
    [Authorize(Policy =Policy.All)]
    public async Task<IActionResult> GetProtocolYearsAsync()
    {
        var protocol = await protocolYearGetterService.GetProtocolYearsAsync();
        if (protocol is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<IEnumerable<ProtocolDtoYears>>.Success(protocol));
    }

    [HttpPost]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> AddProtocolAsync([FromBody] ProtocolDtoAdd protocolDto)
    {
        var addError = await protocolAdderService.AddProtocolAsync(protocolDto);

        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{protocolNumber:regex(^\\d{{4}}$)}/{year:regex(^\\d{{4}}$)}")]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> UpdateProtocolAsync([FromRoute] string protocolNumber, [FromRoute] string year, [FromBody] ProtocolDtoUpdate protocolDto)
    {
        var protocolId = $"{protocolNumber}/{year}";
        if (protocolId != protocolDto.ProtocolId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await protocolUpdatableService.UpdateProtocolAsync(protocolDto);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }

    [HttpPatch("CashFlow/{cashFlowId}")]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> UpdateCashFlowAsync
    (ProtocolDtoUpdateCashFlow protocolDto, [FromRoute] int cashFlowId)
    {
        if (cashFlowId != protocolDto.CashFlowId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var error = await protocolPatchCashFlowIdService.PatchCashFlowIdAsync(protocolDto);
        if (error.IsNotSuccess())
            return StatusCode(error.StatusCode, ApiResponse<object>.Failure(error.Message!, error.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }

    [HttpPost("SpotSaver")]
    [Authorize(Policy =Policy.Workers)]
    public async Task<IActionResult> SpotSaverAsync([FromBody] int quantity)
    {
        var error = await protocolSpotSaverService.SaveProtocolSpotAsync(quantity);
        if (error.IsNotSuccess())
            return StatusCode(error.StatusCode, ApiResponse<object>.Failure(error.Message!, error.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}