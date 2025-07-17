using AuthService.DTOs;
using AuthService.Models;
using RolesBasedAuthentication.Models;

namespace AuthService.Interfaces
{
    public interface IAuthRepository
    {
        Task<RepositoryResultDto<TokenResponseDto>> RegisterAsync(Register request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<bool> ForgetPasswordAsync(string email);
    }
}
