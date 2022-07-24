using Microsoft.AspNetCore.Identity;

namespace Entities.Domain
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }
    }
}
