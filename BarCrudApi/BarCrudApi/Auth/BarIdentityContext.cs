using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarCrudApi.Auth
{
    public class BarIdentityContext: IdentityDbContext<IdentityUser>
    {
        public BarIdentityContext(DbContextOptions<BarIdentityContext> options) : base(options)
        {
        }
        protected BarIdentityContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
