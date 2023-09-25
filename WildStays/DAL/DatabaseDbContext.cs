using System;
using Microsoft.EntityFrameworkCore;
using WildStays.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace WildStays.DAL { 
    public class DatabaseDbContext : IdentityDbContext { 
        public DatabaseDbContext(DbContextOptions<DatabaseDbContext> options) : base(options) { 
            Database.EnsureCreated(); 
        } 
        public DbSet<Listing> Listings { get; set; }
    } 
}