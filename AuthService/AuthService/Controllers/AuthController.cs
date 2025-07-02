using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.DTOs;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register dto)
        {
            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, false, false);

            if (!result.Succeeded)
                return Unauthorized();

            return Ok();
        }
    }
}
