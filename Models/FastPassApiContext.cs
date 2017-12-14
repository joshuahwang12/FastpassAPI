using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FastpassAPI.Models
{
    public partial class FastPassApiContext : DbContext
    {
        public virtual DbSet<FastPass> FastPass { get; set; }
        public virtual DbSet<Rides> Rides { get; set; }
        public virtual DbSet<Tickets> Tickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=tcp:fastpassapi.database.windows.net,1433;Initial Catalog=FastPassApi;Persist Security Info=False;User ID=sqladmin;Password=Password12;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rides>(entity =>
            {
                entity.Property(e => e.RideDescription)
                    .IsRequired()
                    .HasMaxLength(100);
            });
        }
    }
}
