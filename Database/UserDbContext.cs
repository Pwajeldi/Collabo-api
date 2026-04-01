using Collabo_app.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Collabo_app.Database
{
    public class UserDbContext : IdentityDbContext<ApplicationUser>
    {   
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> AppUsers { get; set; }
    }
}
