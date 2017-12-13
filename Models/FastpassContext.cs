using Microsoft.EntityFrameworkCore;

namespace FastpassAPI.Models
{
    public class FastpassContext : DbContext
    {
        public FastpassContext(DbContextOptions<FastpassContext> options)
            : base(options)
            {
            }
            public DbSet<Ride> Rides {get;set;}
            public DbSet<Ticket> Tickets {get;set;}
    }
}