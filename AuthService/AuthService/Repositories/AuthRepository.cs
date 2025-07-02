using AuthService.DTOs;
using AuthService.Interfaces;

namespace AuthService.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        public string Login(Login login)
        {
            throw new NotImplementedException();
        }

        public string Register(Register register)
        {
            throw new NotImplementedException();
        }
        private string GenerateToken()
        {
            return string.Empty; // Placeholder for token generation logic
        }
    }
}
