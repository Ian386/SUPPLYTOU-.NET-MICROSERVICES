using Microsoft.AspNetCore.Identity;

namespace UserService.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }    // Extra field beyond Identity defaults
        
    }
}
