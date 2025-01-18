using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PLCDataCollector.API.Models;
using PLCDataCollector.Models;

namespace PLCDataCollector.API.Data
{
    public class PLCDataContext : IdentityDbContext<ApplicationUser>
    {
        public PLCDataContext(DbContextOptions<PLCDataContext> options)
            : base(options)
        {
        }

        public DbSet<CollectedData> CollectedData { get; set; }
        public DbSet<PLCDevice> PLCDevices { get; set; }
        public DbSet<AlarmRecord> AlarmRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CollectedData>()
                .HasIndex(d => d.Timestamp);
            
            modelBuilder.Entity<CollectedData>()
                .HasIndex(d => d.DeviceName);

            modelBuilder.Entity<AlarmRecord>()
                .HasIndex(a => a.Timestamp);
            
            modelBuilder.Entity<AlarmRecord>()
                .HasIndex(a => a.DeviceName);

            modelBuilder.Entity<PLCDevice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.IPAddress).IsRequired();
                entity.Property(e => e.Port).IsRequired();
            });
        }
    }
} 