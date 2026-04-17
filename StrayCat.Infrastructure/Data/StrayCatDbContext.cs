using Microsoft.EntityFrameworkCore;
using StrayCat.Domain.Entities;

namespace StrayCat.Infrastructure.Data
{
    public class StrayCatDbContext : DbContext
    {
        public StrayCatDbContext(DbContextOptions<StrayCatDbContext> options) : base(options)
        {
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripDate> TripDates { get; set; }
        public DbSet<TripTag> TripTags { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<TripImage> TripImages { get; set; }
        public DbSet<Highlight> Highlights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("trips");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.Duration).HasMaxLength(50);
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Configure relationship with Organizer
                entity.HasOne(t => t.Organizer)
                      .WithMany(o => o.Trips)
                      .HasForeignKey(t => t.OrganizerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TripDate>(entity =>
            {
                entity.ToTable("trip_dates");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Configure relationship with Trip
                entity.HasOne(td => td.Trip)
                      .WithMany(t => t.TripDates)
                      .HasForeignKey(td => td.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TripTag>(entity =>
            {
                entity.ToTable("trip_tags");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Configure relationship with Trip
                entity.HasOne(tt => tt.Trip)
                      .WithMany(t => t.TripTags)
                      .HasForeignKey(tt => tt.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Organizer>(entity =>
            {
                entity.ToTable("organizers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.AvatarInitial).HasMaxLength(10);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("bookings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CustomerPhoneNo).HasMaxLength(20);
                entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ReferenceCode).IsRequired().HasMaxLength(7).IsFixedLength();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Add unique constraint for ReferenceCode
                entity.HasIndex(e => e.ReferenceCode).IsUnique();
                
                // Configure relationship with Trip
                entity.HasOne(b => b.Trip)
                      .WithMany(t => t.Bookings)
                      .HasForeignKey(b => b.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<TripImage>(entity =>
            {
                entity.ToTable("trip_images");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Configure relationship with Trip
                entity.HasOne(ti => ti.Trip)
                      .WithMany(t => t.TripImages)
                      .HasForeignKey(ti => ti.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Highlight>(entity =>
            {
                entity.ToTable("highlights");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Item).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Configure relationship with Trip
                entity.HasOne(h => h.Trip)
                      .WithMany(t => t.Highlights)
                      .HasForeignKey(h => h.TripId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
