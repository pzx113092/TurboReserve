using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TurboReserve.Models;

namespace TurboReserve.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSety dla nowych modeli
        public DbSet<Models.ServiceProvider> ServiceProviders { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<ScheduleSlot> ScheduleSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Service>()
                .HasOne(s => s.ServiceProvider)
                .WithMany(sp => sp.Services)
                .HasForeignKey(s => s.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.ServiceProvider)
                .WithMany() 
                .HasForeignKey(r => r.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.Service)
                .WithMany() 
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .HasOne(r => r.Customer)
                .WithMany() 
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<CustomerProfile>()
                .HasOne(cp => cp.User)
                .WithOne() 
                .HasForeignKey<CustomerProfile>(cp => cp.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

                builder.Entity<Service>()
            .HasMany(s => s.ScheduleSlots)
            .WithOne(ss => ss.Service)
            .HasForeignKey(ss => ss.ServiceId);

                builder.Entity<Reservation>()
            .HasOne(r => r.Service)
            .WithMany(s => s.Reservations)
            .HasForeignKey(r => r.ServiceId);


                builder.Entity<Service>()
            .HasMany(s => s.Reservations)
            .WithOne(r => r.Service)
            .HasForeignKey(r => r.ServiceId);
        }
    }
}
