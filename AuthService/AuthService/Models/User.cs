using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    public class User : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
