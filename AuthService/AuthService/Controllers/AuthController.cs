using Microsoft.AspNetCore.Mvc;
using AuthService.DTOs;
using AuthService.Interfaces;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register dto)
        {
            var tokens = _authRepository.Register(dto);
            return Ok(tokens);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login dto)
        {
            var tokens = _authRepository.Login(dto);
            return Ok(tokens);
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] string refreshToken)
        {
            var tokens = _authRepository.RefreshToken(refreshToken);
            return Ok(tokens);
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] string email)
        {
            var result = await _authRepository.ForgetPasswordAsync(email);
            if (!result)
                return NotFound("User not found");
            return Ok("Password reset instructions sent to your email.");
        }
    }
}
