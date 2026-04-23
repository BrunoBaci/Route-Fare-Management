using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Route_Fare_Management.Application.Interfaces;

namespace Route_Fare_Management.Infrastructure.Services
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql =>
                    {
                        sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                        sql.CommandTimeout(60);
                    }));

            // -------------------- Repository --------------------
            services.AddScoped<IRepository, PersistenceService>();

            // -------------------- Security --------------------
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

            // -------------------- User Context --------------------
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // -------------------- Export --------------------
            services.AddScoped<IExportService, ExcelExportService>();

            //  Notifications (SignalR abstraction) 
            services.AddScoped<INotificationService, SignalRNotificationService>();

            return services;
        
            /// <summary>
            /// Applies any pending EF Core migrations.
            /// Called from Program.cs after app.Build() 
            /// </summary>
            //public static async Task ApplyMigrationsAsync(IServiceProvider services)
            //{
            //    using var scope = services.CreateScope();
            //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //    // InMemory database does not support migrations
            //    if (db.Database.IsInMemory()) return;

            //    await db.Database.MigrateAsync();
            //}
            }
    }
}