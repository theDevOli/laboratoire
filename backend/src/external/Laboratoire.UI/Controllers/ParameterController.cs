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
[Authorize(Policy =Policy.Workers)]
public class ParameterController
(
    IParameterGetterService parameterGetterService,
    IParameterInputGetterService parameterInputGetterService,
    IParameterGetterByIdService parameterGetterByIdService,
    IParameterAdderService parameterAdderService,
    IParameterUpdatableService parameterUpdatableService
)
: ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllParametersAsync()
    {
        var parameters = await parameterGetterService.GetAllParametersAsync();
        return Ok(ApiResponse<IEnumerable<Parameter>>.Success(parameters));
    }

    [HttpGet("Input/{catalogId}")]
    public async Task<IActionResult> GetParameterInputByIdAsync([FromRoute] int catalogId)
    {
        var parameter = await parameterInputGetterService.GetParameterInputByIdAsync(catalogId);
        if (parameter is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<IEnumerable<ParameterDtoDisplay>>.Success(parameter));
    }

    [HttpGet("{parameterId}")]
    public async Task<IActionResult> GetParameterByIdAsync([FromRoute] int parameterId)
    {
        var parameter = await parameterGetterByIdService.GetParameterByIdAsync(parameterId);
        if (parameter is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Parameter>.Success(parameter));
    }

    [HttpPost]
    public async Task<IActionResult> AddParameterAsync([FromBody] ParameterDtoAdd parameterDto)
    {
        var addError = await parameterAdderService.AddParameterAsync(parameterDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{parameterId}")]
    public async Task<IActionResult> UpdateParameterAsync([FromRoute] int parameterId, [FromBody] Parameter parameter)
    {
        if (parameterId != parameter.ParameterId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await parameterUpdatableService.UpdateParameterAsync(parameter);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Added));
    }
}