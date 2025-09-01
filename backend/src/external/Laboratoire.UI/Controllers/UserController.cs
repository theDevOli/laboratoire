using Microsoft.AspNetCore.Mvc;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.Utils;
using Laboratoire.Application.DTO;
using Laboratoire.Application.ServicesContracts;
using Microsoft.AspNetCore.Authorization;

namespace Laboratoire.UI.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class UserController
(
    IUserGetterService userGetterService,
    IUserGetterByIdService userGetterByIdService,
    IUserAdderService userAdderService,
    IUserUpdatableService userUpdatableService,
    IUserPatchService userPatchService,
    IUserRenameService userRenameService,
    IAuthenticationGetterService authenticationGetterService
)
: ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "admin,recepção")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await userGetterService.GetAllUsersAsync();
        return Ok(ApiResponse<IEnumerable<DisplayUser>>.Success(users));
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "admin,recepção")]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid userId)
    {
        var user = await userGetterByIdService.GetUserByIdAsync(userId);
        if (user is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<User>.Success(user));
    }

    [HttpGet("Authentication/{userId}")]
    [Authorize(Policy = "all")]
    public async Task<IActionResult> GetAuthenticationByUserIdAsync([FromRoute] Guid userId)
    {
        var authentication = await authenticationGetterService.GetAuthenticationByUserId(userId);
        if (authentication is null)
            return NotFound(ApiResponse<object>.Failure(ErrorMessage.NotFound, 404));

        return Ok(ApiResponse<Authentication>.Success(authentication));
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddUserAsync([FromBody] UserDtoAdd userDto)
    {
        var userId = await userAdderService.AddUserAsync(userDto);
        if (userId is null)
            return StatusCode(500, ApiResponse<object>.Failure(ErrorMessage.DbError, 500));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPut("{userId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUserByIdAsync([FromRoute] Guid userId, [FromBody] User user)
    {
        if (userId != user.UserId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));

        var updateError = await userUpdatableService.UpdateUserAsync(user);
        if (updateError.IsNotSuccess())
            return StatusCode(updateError.StatusCode, ApiResponse<object>.Failure(updateError.Message!, updateError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Added));
    }

    [HttpPatch("{userId}/Status")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUserStatusAsync([FromRoute] Guid userId)
    {
        var patchError = await userPatchService.UpdateUserStatusAsync(userId);

        if (patchError.IsNotSuccess())
            return StatusCode(patchError.StatusCode, ApiResponse<object>.Failure(patchError.Message!, patchError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }

    [HttpPatch("{userId}/Rename")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUserStatusAsync([FromRoute] Guid userId, UserDtoRename userDto)
    {
        if (userId != userDto.UserId)
            return BadRequest(ApiResponse<object>.Failure(ErrorMessage.BadRequestID, 400));
        var patchError = await userRenameService.UserRenameAsync(userDto);

        if (patchError.IsNotSuccess())
            return StatusCode(patchError.StatusCode, ApiResponse<object>.Failure(patchError.Message!, patchError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Updated));
    }
}