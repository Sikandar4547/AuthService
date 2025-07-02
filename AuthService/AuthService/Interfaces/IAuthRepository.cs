using AuthService.DTOs;

namespace AuthService.Interfaces
{
    public interface IAuthRepository
    {
        string Register(Register register);
        string Login(Login login);
    }
}
