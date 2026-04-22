using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Route_Fare_Management.Application;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    

        public DbSet<User> Users => Set<User>();
        public DbSet<TourOperator> TourOperators => Set<TourOperator>();
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<Season> Seasons => Set<Season>();
        public DbSet<TourOperatorRoute> TourOperatorRoutes => Set<TourOperatorRoute>();
        public DbSet<PricingEntry> PricingEntries => Set<PricingEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PricingEntry>(b =>
            {
                b.ToTable("PricingEntries");
                b.HasKey(p => p.Id);

                b.HasIndex(p => new { p.TourOperatorRouteId, p.Date }).IsUnique();
                b.HasIndex(p => p.Date);

                b.Property(p => p.Date).IsRequired();
                b.Property(p => p.DayOfWeek).HasConversion<int>().IsRequired();

                b.Property(p => p.EconomyPrice).HasColumnType("decimal(18,2)");
                b.Property(p => p.BusinessPrice).HasColumnType("decimal(18,2)");
                b.Property(p => p.FirstClassPrice).HasColumnType("decimal(18,2)");

                b.Property(p => p.CreatedAt).IsRequired();
                b.Property(p => p.UpdatedAt);

                b.HasOne(p => p.TourOperatorRoute)
                 .WithMany(t => t.PricingEntries)
                 .HasForeignKey(p => p.TourOperatorRouteId)
                 .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Route>(b => {
                b.ToTable("Routes");
                b.HasKey(r => r.Id);

                b.Property(r => r.Origin).IsRequired().HasMaxLength(100);
                b.Property(r => r.Destination).IsRequired().HasMaxLength(100);
                b.Property(r => r.Description).HasMaxLength(500);
                b.HasIndex(r => new { r.Origin, r.Destination }).IsUnique();
                b.Property(r => r.IsActive).IsRequired().HasDefaultValue(true);
                b.Property(r => r.CreatedAt).IsRequired();
                b.Property(r => r.UpdatedAt);

                var converter = new ValueConverter<List<BookingClass>, string>(
                    list => string.Join(',', list.Select(bc => (int)bc)),
                    csv => string.IsNullOrWhiteSpace(csv)
                        ? new List<BookingClass>()
                        : csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => (BookingClass)int.Parse(s))
                             .ToList());

                b.Property<List<BookingClass>>("_availableBookingClasses")
                 .HasField("_availableBookingClasses")
                 .UsePropertyAccessMode(PropertyAccessMode.Field)
                 .HasColumnName("AvailableBookingClasses")
                 .HasConversion(converter)
                 .HasMaxLength(50)
                 .IsRequired()
                 .HasDefaultValue(new List<BookingClass>());
            });


            modelBuilder.Entity<Season>(b =>
            {
                b.ToTable("Seasons");
                b.HasKey(s => s.Id);

                b.Property(s => s.Year).IsRequired();
                b.Property(s => s.Type).HasConversion<int>().IsRequired();
                b.Property(s => s.StartDate).IsRequired();
                b.Property(s => s.EndDate).IsRequired();
                b.HasIndex(s => new { s.Year, s.Type }).IsUnique();
                b.Property(s => s.CreatedAt).IsRequired();
                b.Property(s => s.UpdatedAt);

                // Computed properties — not stored in DB
                b.Ignore(s => s.DisplayName);
                b.Ignore(s => s.TotalDays);

            });

            modelBuilder.Entity<TourOperator>(b => 
            {
                b.ToTable("TourOperators");
                b.HasKey(t => t.Id);

                b.Property(t => t.Name).IsRequired().HasMaxLength(200);
                b.Property(t => t.Code).IsRequired().HasMaxLength(10);
                b.HasIndex(t => t.Code).IsUnique();
                b.Property(t => t.IsActive).IsRequired().HasDefaultValue(true);
                b.Property(t => t.CreatedAt).IsRequired();
                b.Property(t => t.UpdatedAt);

                // Store List<BookingClass> as comma-separated integers: "1,2,3"
                // EF reads/writes via the private backing field using reflection
                var converter = new ValueConverter<List<BookingClass>, string>(
                    list => string.Join(',', list.Select(bc => (int)bc)),
                    csv => string.IsNullOrWhiteSpace(csv)
                        ? new List<BookingClass>()
                        : csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => (BookingClass)int.Parse(s))
                             .ToList());

                b.Property<List<BookingClass>>("_supportedBookingClasses")
                 .HasField("_supportedBookingClasses")
                 .UsePropertyAccessMode(PropertyAccessMode.Field)
                 .HasColumnName("SupportedBookingClasses")
                 .HasConversion(converter)
                 .HasMaxLength(50)
                 .IsRequired()
                 .HasDefaultValue(new List<BookingClass>());
            });

            modelBuilder.Entity<TourOperatorRoute>(b => {
                b.ToTable("TourOperatorRoutes");
                b.HasKey(t => t.Id);

                // Composite unique constraint — one assignment per operator/route/season
                b.HasIndex(t => new { t.TourOperatorId, t.RouteId, t.SeasonId }).IsUnique();
                b.Property(t => t.CreatedAt).IsRequired();
                b.Property(t => t.UpdatedAt);

                b.HasOne(t => t.TourOperator)
                 .WithMany(op => op.TourOperatorRoutes)
                 .HasForeignKey(t => t.TourOperatorId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(t => t.Route)
                 .WithMany(r => r.TourOperatorRoutes)
                 .HasForeignKey(t => t.RouteId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(t => t.Season)
                 .WithMany(s => s.TourOperatorRoutes)
                 .HasForeignKey(t => t.SeasonId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(b => { });
            }

        public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
        {
            // 1. Collect domain events BEFORE saving (entities may be detached after)
            var entitiesWithEvents = ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            // 2. Auto-stamp UpdatedAt on every modified entity
            foreach (var entry in ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Modified))
            {
                entry.Entity.SetUpdatedAt();
            }

            // 3. Persist
            var result = await base.SaveChangesAsync(cancellationToken);

            // 4. Clear events after successful persist
            foreach (var entity in entitiesWithEvents)
                entity.ClearDomainEvents();

            return result;
        }
    } 
}
