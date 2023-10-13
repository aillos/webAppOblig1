using Microsoft.EntityFrameworkCore;
using WildStays.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WildStays.DAL
{
    //Uses ApplicationUser instead of IdentityUser per the default.
    public class DatabaseDbContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseDbContext(DbContextOptions<DatabaseDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}
