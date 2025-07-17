using AuthService.DTOs;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RolesBasedAuthentication.Models;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthRepository authRepository) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> Register(Register request)
        {
            RepositoryResultDto<TokenResponseDto> result = await authRepository.RegisterAsync(request);
            var response = new ApiResponseDto<TokenResponseDto>
            {
                StatusCode = result.ErrorMessage == null ? ApiStatusCode.OK : ApiStatusCode.BadRequest,
                ErrorMessage = result.ErrorMessage,
                SuccessMessage = result.SuccessMessage,
                Data = result.Data
            };
            return result.ErrorMessage == null ? Ok(response) : BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> Login(UserDto request)
        {
            var result = await authRepository.LoginAsync(request);
            var response = new ApiResponseDto<TokenResponseDto>
            {
                StatusCode = result == null ? ApiStatusCode.BadRequest : ApiStatusCode.OK,
                ErrorMessage = result == null ? "Invalid username or password." : null,
                SuccessMessage = result != null ? "Login successful." : null,
                Data = result
            };
            return result == null ? BadRequest(response) : Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authRepository.RefreshTokenAsync(request);
            var response = new ApiResponseDto<TokenResponseDto>
            {
                StatusCode = (result == null || result.AccessToken == null || result.RefreshToken == null)
                    ? ApiStatusCode.Unauthorized
                    : ApiStatusCode.OK,
                ErrorMessage = (result == null || result.AccessToken == null || result.RefreshToken == null)
                    ? "Invalid refresh token."
                    : null,
                SuccessMessage = (result != null && result.AccessToken != null && result.RefreshToken != null)
                    ? "Token refreshed."
                    : null,
                Data = result
            };
            return (result == null || result.AccessToken == null || result.RefreshToken == null)
                ? Unauthorized(response)
                : Ok(response);
        }

        [HttpPost("forget-password")]
        public async Task<ActionResult<ApiResponseDto<object>>> ForgetPassword([FromBody] string email)
        {
            var result = await authRepository.ForgetPasswordAsync(email);
            var response = new ApiResponseDto<object>
            {
                StatusCode = result ? ApiStatusCode.OK : ApiStatusCode.NotFound,
                ErrorMessage = result ? null : "User not found",
                SuccessMessage = result ? "Password reset instructions sent to your email." : null,
                Data = null
            };
            return result ? Ok(response) : NotFound(response);
        }

        [Authorize]
        [HttpGet]
        public ActionResult<ApiResponseDto<string>> AuthenticatedOnlyEndpoint()
        {
            var response = new ApiResponseDto<string>
            {
                StatusCode = ApiStatusCode.OK,
                SuccessMessage = "You are authenticated!",
                Data = "You are authenticated!"
            };
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public ActionResult<ApiResponseDto<string>> AdminOnlyEndpoint()
        {
            var response = new ApiResponseDto<string>
            {
                StatusCode = ApiStatusCode.OK,
                SuccessMessage = "You are an admin!",
                Data = "You are an admin!"
            };
            return Ok(response);
        }
    }
}
