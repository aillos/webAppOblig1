using System;
using Microsoft.EntityFrameworkCore;

namespace WildStays.Models
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
		{
			Database.EnsureCreated();
		}
        public DbSet<Listing> Listings { get; set; }
    }
}

