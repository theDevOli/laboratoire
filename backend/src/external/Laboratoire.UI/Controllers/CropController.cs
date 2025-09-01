using Microsoft.AspNetCore.Mvc;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;
using Laboratoire.Domain.Utils;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
[Authorize(Policy = Policy.Workers)]
public class CropController
(
    ICropGetterService cropGetterService,
    ICropGetterByIdService cropGetterByIdService,
    ICropAdderService cropAdderService,
    ICropUpdatableService cropUpdatableService
)
: ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllCropsAsync()
    {
        var crops = await cropGetterService.GetAllCropsAsync();
        return Ok(ApiResponse<IEnumerable<Crop>>.Success(crops));
    }

    [HttpGet("{cropId}")]
    public async Task<IActionResult> GetCropByIdAsync([FromRoute] int cropId)
    {
        var crop = await cropGetterByIdService.GetCropByIdAsync(cropId);
        if (crop is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Crop>.Success(crop));
    }

    [HttpPost]
    public async Task<IActionResult> AddCropAsync([FromBody] CropDtoAdd cropDto)
    {
        var addError = await cropAdderService.AddCropAsync(cropDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{cropId}")]
    public async Task<IActionResult> UpdateCropAsync([FromBody] Crop crop, [FromRoute] int cropId)
    {
        if (cropId != crop.CropId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await cropUpdatableService.UpdateCropAsync(crop);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Added));
    }
}