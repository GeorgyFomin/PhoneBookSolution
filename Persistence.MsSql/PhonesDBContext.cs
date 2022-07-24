using Entities.Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence.MsSql
{
    public class PhonesDBContext : DbContext
    {
        public PhonesDBContext(DbContextOptions<PhonesDBContext> options)
            : base(options)
        {
        }
        public DbSet<Phone> Phones { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Phone>().OwnsOne(p => p.PhoneNumder);
        }
    }
}
