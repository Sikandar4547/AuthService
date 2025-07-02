using AuthService.DTOs;

namespace AuthService.Interfaces
{
    public interface IAuthRepository
    {
        TokenResponse Register(Register register);
        TokenResponse Login(Login login);
        TokenResponse RefreshToken(string refreshToken);
        Task<bool> ForgetPasswordAsync(string email);
    }
}
