using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Route_Fare_Management.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Route_Fare_Management.Infrastructure.Repositories;

namespace Route_Fare_Management.Infrastructure.Services
{
    /// <summary>
    /// This class will be used to register infrastructure dependencies 
    /// without referring to infrastructure in API layer
    /// </summary>
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //  SQL Server 
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(
                    configuration.GetConnectionString("ConnectionString"),
                    sql =>
                    {
                        sql.MigrationsAssembly(
                            typeof(AppDbContext).Assembly.FullName);
                        sql.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                        sql.CommandTimeout(60);
                    }));

            services.AddScoped<IRepository>(sp =>
                sp.GetRequiredService<Repository>());

            services.AddScoped<AppDbContext>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Export
            services.AddScoped<IExportService, ExcelExportService>();
            // SignalR
            services.AddScoped<INotificationService, SignalRNotificationService>();

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly",
                    p => p.RequireRole("Admin"))
                .AddPolicy("OperatorOrAdmin",
                    p => p.RequireRole("Admin", "TourOperatorMember"));

            return services;
        }

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
