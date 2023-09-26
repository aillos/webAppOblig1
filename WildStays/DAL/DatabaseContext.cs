using Microsoft.EntityFrameworkCore;
using WildStays.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WildStays.DAL { 
    public class DatabaseContext : IdentityDbContext { 
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { 
            Database.EnsureCreated(); 
        } 
        public DbSet<Listing> Listings { get; set; }
    } 
}