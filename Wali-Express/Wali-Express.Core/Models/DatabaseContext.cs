using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Wali_Express.Core.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

    }
}