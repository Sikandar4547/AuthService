using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.DTOs;
using AuthService.Interfaces;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
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

        public TokenResponse Register(Register register)
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

            var jwtToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken(user);

            return new TokenResponse
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken
            };
        }

        public TokenResponse Login(Login login)
        {
            var user = _userManager.FindByNameAsync(login.Username).GetAwaiter().GetResult()
                ?? _userManager.FindByEmailAsync(login.Username).GetAwaiter().GetResult();

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var result = _signInManager.CheckPasswordSignInAsync(user, login.Password, false).GetAwaiter().GetResult();
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var jwtToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken(user);

            return new TokenResponse
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken
            };
        }

        public TokenResponse RefreshToken(string refreshToken)
        {
            var username = ValidateRefreshToken(refreshToken);
            if (username == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var jwtToken = GenerateJwtToken(username);
            var newRefreshToken = GenerateRefreshToken(username);

            return new TokenResponse
            {
                JwtToken = jwtToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> ForgetPasswordAsync(string email)
        {
            var user = await FindUserByEmailAsync(email);
            if (user == null) return false;

            var resetToken = GeneratePasswordResetToken(user);
            await SendResetEmailAsync(email, resetToken);
            return true;
        }
        private Task<object> FindUserByEmailAsync(string email) => Task.FromResult<object>(null);
        private string GeneratePasswordResetToken(object user) => "reset-token";
        private Task SendResetEmailAsync(string email, string token) => Task.CompletedTask;

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
                expires: DateTime.UtcNow.AddMinutes(5),
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(User user)
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
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string? ValidateRefreshToken(string refreshToken)
        {
            if (refreshToken.StartsWith("refresh-token-for-"))
                return refreshToken.Replace("refresh-token-for-", "");
            return null;
        }
    }
}
