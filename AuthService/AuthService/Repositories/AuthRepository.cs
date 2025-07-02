using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.DTOs;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthRepository(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public string Register(Register register)
        {
            var user = new User
            {
                UserName = register.Username,
                Email = register.Email,
                FirstName = register.FirstName,
                LastName = register.LastName
            };

            var result = _userManager.CreateAsync(user, register.Password).GetAwaiter().GetResult();
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return GenerateToken(user);
        }

        public string Login(Login login)
        {
            var user = _userManager.FindByNameAsync(login.Username).GetAwaiter().GetResult()
                ?? _userManager.FindByEmailAsync(login.Username).GetAwaiter().GetResult();

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var result = _signInManager.CheckPasswordSignInAsync(user, login.Password, false).GetAwaiter().GetResult();
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials.");

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJwtTokenGeneration!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(1),
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
