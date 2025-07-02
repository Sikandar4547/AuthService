using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.DTOs;
using Microsoft.AspNetCore.Identity;
using AuthService.Interfaces;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthRepository _authRepository;
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager,IAuthRepository authRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register dto)
        {
            _authRepository.Register(dto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login dto)
        {
            _authRepository.Login(dto);
            return Ok();
        }
    }
}
