using Microsoft.EntityFrameworkCore;

namespace FastpassAPI.Models
{
    public class FastpassContext : DbContext
    {
        public FastpassContext(DbContextOptions<FastpassContext> options)
            : base(options)
            {

            }

            public DbSet<FastPass> FastPass {get;set;}
    }
}