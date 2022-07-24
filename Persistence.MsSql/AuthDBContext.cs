using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.MsSql
{
    public class AuthDBContext : IdentityDbContext<IdentityUser>
    {
        public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options) { }
    }
}
