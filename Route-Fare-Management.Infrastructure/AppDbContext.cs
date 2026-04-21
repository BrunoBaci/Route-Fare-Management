using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
