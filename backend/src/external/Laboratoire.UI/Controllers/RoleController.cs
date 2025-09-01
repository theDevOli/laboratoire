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
public class RoleController
(
    IRoleGetterService roleGetterService,
    IRoleGetterByIdService roleGetterByIdService,
    IRoleAdderService roleAdderService,
    IRoleUpdatableService roleUpdatableService
) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        var roles = await roleGetterService.GetAllRolesAsync();
        return Ok(ApiResponse<IEnumerable<Role>>.Success(roles));
    }

    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetRoleByIdAsync([FromRoute] int roleId)
    {
        var role = await roleGetterByIdService.GetRoleByIdAsync(roleId);
        if (role is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Role>.Success(role));
    }

    [HttpPost]
    public async Task<IActionResult> AddRoleAsync([FromBody] RoleDtoAdd roleDto)
    {
        var addError = await roleAdderService.AddRoleAsync(roleDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{roleId}")]
    public async Task<IActionResult> UpdateRoleAsync([FromRoute] int roleId, [FromBody] Role role)
    {
        if (roleId != role.RoleId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await roleUpdatableService.UpdateRoleAsync(role);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}