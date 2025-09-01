using Microsoft.AspNetCore.Mvc;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
[Authorize(Roles = "admin,químico")]
public class HazardController
(
    IHazardGetterService hazardGetterService,
    IHazardGetterByIdService hazardGetterByIdService,
    IHazardAdderService hazardAdderService,
    IHazardUpdatableService hazardUpdatable
)
: ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllHazard()
    {
        var hazards = await hazardGetterService.GetAllHazardsAsync();
        return Ok(ApiResponse<IEnumerable<Hazard>>.Success(hazards));
    }

    [HttpGet("{hazardId}")]
    public async Task<IActionResult> GetHazardByIdAsync([FromRoute] int hazardId)
    {
        var hazard = await hazardGetterByIdService.GetHazardByIdAsync(hazardId);
        if (hazard is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Hazard>.Success(hazard));
    }

    [HttpPost]
    public async Task<IActionResult> AddHazardAsync([FromBody] HazardDtoAdd hazardDto)
    {
        var addError = await hazardAdderService.AddHazardAsync(hazardDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{hazardId}")]
    public async Task<IActionResult> UpdateHazardAsync([FromRoute] int hazardId, [FromBody] Hazard hazard)
    {
        if (hazardId != hazard.HazardId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await hazardUpdatable.UpdateHazardAsync(hazard);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}