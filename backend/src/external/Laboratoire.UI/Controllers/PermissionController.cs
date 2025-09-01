using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Laboratoire.Application.Utils;
using Laboratoire.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class PermissionController
(
    IPermissionAdderService permissionAdderService,
    IPermissionGetterService permissionGetterService,
    IPermissionUpdatableService permissionUpdatableService
)
 : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "admin,recepção")]
    public async Task<IActionResult> GetAllPermissionsAsync()
    {
        var permissions = await permissionGetterService.GetAllPermissionsAsync();
        return Ok(ApiResponse<IEnumerable<DisplayPermission>>.Success(permissions));
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddPermissionAsync(PermissionDtoAdd dto)
    {
        var error = await permissionAdderService.AddPermissionAsync(dto);
        if (error.IsNotSuccess())
            return StatusCode(error.StatusCode, ApiResponse<object>.Failure(error.Message!, error!.StatusCode));

        return StatusCode(201, ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{permissionId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdatePermissionAsync([FromRoute] int permissionId, Permission permission)
    {
        if (permissionId != permission.PermissionId)
            return Conflict(ApiResponse<object>.Failure(ErrorMessage.ConflictPut, 409));

        var error = await permissionUpdatableService.UpdatePermissionAsync(permission);
        if (error.IsNotSuccess())
            return StatusCode(error.StatusCode, ApiResponse<object>.Failure(error.Message!, error!.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}