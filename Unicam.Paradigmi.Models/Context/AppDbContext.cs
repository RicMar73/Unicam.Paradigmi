using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Unicam.Paradigmi.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Unicam.Paradigmi.Models.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Booking>().HasIndex(b => new { b.ResourceId, b.Start, b.End }).IsUnique();

            modelBuilder.Entity<Booking>()
           .HasOne(b => b.Resource) // Ogni Booking ha una risorsa
           .WithMany()  // Ogni Resource può avere molte prenotazioni
           .HasForeignKey(b => b.ResourceId); // La chiave esterna è ResourceId
        }
    }

}
