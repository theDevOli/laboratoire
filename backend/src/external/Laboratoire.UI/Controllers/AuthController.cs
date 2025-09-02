using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Laboratoire.Domain.Entity;
using Laboratoire.Application.DTO;
using Laboratoire.Application.Utils;
using Laboratoire.Application.ServicesContracts;

namespace Laboratoire.UI.Controllers;

[Authorize]
[ApiController]
[Route("v1/api/[controller]")]
public class AuthController
(
    IAuthLoginService authLoginService,
    IAuthTokenRefresherService authTokenRefresherService,
    IAuthChangePasswordService authChangePasswordService,
    IAuthResetPasswordService authResetPasswordService,
    IAuthTokenValidatorService authTokenValidatorService,
    IUserGetterByUsernameService userGetterByUsernameService,
    IRoleGetterByUserIdService roleGetterByUserIdService,

    Token token
) : ControllerBase
{

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLogin userLogin)
    {

        var addError = await authLoginService.LoginUserAsync(userLogin);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        var user = await userGetterByUsernameService.GetUserByUsernameAsync(userLogin.Username);
        var userId = user?.UserId;
        var role = await roleGetterByUserIdService.GetRoleNameByUserIdAsync(userId);
        var createdToken = token.CreateToken(userId, role);

        var tokenResponse = new AuthDtoToken() { Token = createdToken };

        return Ok(ApiResponse<AuthDtoToken>.Success(tokenResponse));

    }

    [HttpPost("RefreshToken")]
    public IActionResult RefreshTokenAsync([FromBody] AuthDtoRefreshToken authDto)
    {
        var token = authTokenRefresherService.RefreshToken(authDto);
        if (token is null)
            return Unauthorized(ApiResponse<object>.Failure(ErrorMessage.Unauthorized, 401));
        // var jwtTokenHandler = new JwtSecurityTokenHandler();
        // var jwtToken = jwtTokenHandler.ReadJwtToken(authDto.RefreshToken);
        // var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId");
        // var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role");

        // var userId = Guid.Parse(userIdClaim?.Value!);
        // var role = roleClaim?.Value;

        // var createdToken = token.RefreshToken(authDto);
        var tokenResponse = new AuthDtoToken() { Token = token };

        return Ok(ApiResponse<AuthDtoToken>.Success(tokenResponse));
    }

    [HttpPost("ValidateToken")]
    public IActionResult ValidateToken([FromBody] AuthDtoToken token)
    {
        var isValid = authTokenValidatorService.ValidateTokenAsync(token);
        if (!isValid)
            return Unauthorized(ApiResponse<object>.Failure(ErrorMessage.Unauthorized, 401));
        // var jwtTokenHandler = new JwtSecurityTokenHandler();
        // var jwtToken = jwtTokenHandler.ReadJwtToken(authDto.RefreshToken);
        // var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId");
        // var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role");

        // var userId = Guid.Parse(userIdClaim?.Value!);
        // var role = roleClaim?.Value;

        return Ok(ApiResponse<AuthDtoToken>.Success(token));
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] UserDtoChangePassword userDto)
    {
        var addError = await authChangePasswordService.ChangeUserPasswordAsync(userDto);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Login));
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync(AuthDtoResetPassword dto)
    {

        var userId = dto.UserId;
        var addError = await authResetPasswordService.ResetPasswordAsync(userId);
        if (addError.IsNotSuccess())
            return StatusCode(addError.StatusCode, ApiResponse<object>.Failure(addError.Message!, addError.StatusCode));

        return Ok(ApiResponse<string>.Success(SuccessMessage.Login));
    }
}