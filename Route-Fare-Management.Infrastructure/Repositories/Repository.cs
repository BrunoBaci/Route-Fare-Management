using MediatR;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Domain;
using Route_Fare_Management.Domain.Exceptions;
using System.Linq;
using Azure.Core;
using Route_Fare_Management.Application.PricingFunctionality.DTOs;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Routing;

namespace Route_Fare_Management.Infrastructure.Repositories
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {

            await _context.SaveChangesAsync(token);
        }

        /// <summary>
        /// Gets a user by searching his email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<User> GetUserAsync(string email, CancellationToken token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                   u => u.Email == email.ToLowerInvariant() && u.IsActive,
                   token);
            return user;
        }

        public async Task<int> AddAndSaveAsync(User user, CancellationToken token)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync(token);
        }
        public async Task<int> AddAndSaveAsync(Season season, CancellationToken token)
        {
            await _context.Seasons.AddAsync(season, token);
            return await _context.SaveChangesAsync(token);
        }
        public async Task<int> AddAndSaveAsync(Domain.Route route, CancellationToken token)
        {
            await _context.Routes.AddAsync(route, token);
            return await _context.SaveChangesAsync(token);
        }
        public async Task<int> AddAndSaveAsync(TourOperatorRoute tourOperatorRoute, CancellationToken token)
        {
            await _context.TourOperatorRoutes.AddAsync(tourOperatorRoute, token);
            return await _context.SaveChangesAsync(token);
        }

        public async Task AddAsync(TourOperatorRoute tourOperatorRoute, CancellationToken token)
        {
            await _context.TourOperatorRoutes.AddAsync(tourOperatorRoute, token);
        }

        public async Task AddAsync(PricingEntry route, CancellationToken token)
        {
           await  _context.PricingEntries.AddAsync(route, token);
        }

        public async Task AddAsync(TourOperator entity, CancellationToken ct)
        {
            await _context.TourOperators.AddAsync(entity, ct);
        }

        public Task<TourOperator?> GetTourOperatorWithMembersAsync(Guid id, CancellationToken ct)
        {
            return _context.TourOperators
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<Domain.Route?> GetRouteAsync(Guid id, CancellationToken token)
        {
            return await _context.Routes
            .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, token);
        }

        public async Task<List<Domain.Route?>> GetRoutesAsync(string? term, CancellationToken token)
        {
            var query = _context.Routes
                .AsNoTracking()
                .Where(r => r.IsActive);

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(r =>
                    r.Origin.ToLower().Contains(term) ||
                    r.Destination.ToLower().Contains(term));
            }

            var routes = await query
                .OrderBy(r => r.Origin)
                .ThenBy(r => r.Destination)
                .ToListAsync(token);

            return routes;
        }

        public async Task<Domain.Route?> UpdateRouteAsync(Guid id,
            string origin,
            string destination,
            string description,
            List<BookingClass> bookingClasses,
            CancellationToken token)
        {
            var route = await _context.Routes
                .FindAsync(new object[] { id }, token);
            
            if(route != null)
                route.Update(
                origin,
                destination,
                description,
                bookingClasses);
            await _context.SaveChangesAsync(token);
            return route;
        }

        public async Task<TourOperatorRoute?> GetForPricingInitializationAsync(Guid tourOperatorId,
            Guid routeId,
            Guid seasonId,
            CancellationToken cancellationToken)
        {
            return await _context.TourOperatorRoutes
             .Include(t => t.TourOperator).ThenInclude(o => o.Members)
             .Include(t => t.Route)
             .Include(t => t.Season)
             .Include(t => t.PricingEntries)
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                 t.TourOperatorId == tourOperatorId &&
                 t.RouteId == routeId &&
                 t.SeasonId == seasonId,
                 cancellationToken);
        }

        public async Task<TourOperatorRoute?> GetWithPricingAsync(
        Guid tourOperatorId,
        Guid routeId,
        Guid seasonId,
        CancellationToken cancellationToken)
        {
            return await _context.TourOperatorRoutes
                .Include(t => t.PricingEntries)
                .FirstOrDefaultAsync(t =>
                    t.TourOperatorId == tourOperatorId &&
                    t.RouteId == routeId &&
                    t.SeasonId == seasonId,
                    cancellationToken);
        }

        public async Task AddPricingEntriesAsync(IEnumerable<PricingEntry> entries, CancellationToken ct)
        {
            await _context.PricingEntries.AddRangeAsync(entries, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Season?> GetSeasonAsync(Guid Id, CancellationToken token)
        {
            var season = await _context.Seasons
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == Id, token);
            return season;
        }

        public async Task<List<Season>> GetSeasonsAsync(int? year, CancellationToken cancellationToken)
        {
            var query = _context.Seasons.AsNoTracking();

            if (year.HasValue)
                query = query.Where(s => s.Year == year.Value);

            var seasons = await query
                .OrderByDescending(s => s.Year)
                .ThenBy(s => s.Type)
                .ToListAsync(cancellationToken);

            return seasons;
        }

        public Task<bool> ExistsTourOperatorRouteAsync(Guid tourOperatorId, Guid routeId, Guid seasonId, CancellationToken ct)
        {
            return _context.TourOperatorRoutes
                .AnyAsync(t =>
                    t.TourOperatorId == tourOperatorId &&
                    t.RouteId == routeId &&
                    t.SeasonId == seasonId,
                    ct);
        }
        public Task<TourOperator?> GetTourOperatorAsync(Guid id, CancellationToken ct)
        {
            return _context.TourOperators
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<List<TourOperator>> GetTourOperatorsAsync(CancellationToken ct, bool activeOnly = false)
        {
            var query = _context.TourOperators
                .Include(t => t.Members)
                .AsNoTracking();

            if (activeOnly == true)
                query = query.Where(t => t.IsActive);

            var entities = await query
                .OrderBy(t => t.Name)
                .ToListAsync(ct);

            return entities;

        }

        public async Task<List<TourOperatorRoute>> GetOperatorRoutesAsync(
            Guid tourOperatorId,
            Guid? seasonId,
            CancellationToken cancellationToken)
        {
            var query = _context.TourOperatorRoutes
                .Include(t => t.TourOperator)
                    .ThenInclude(o => o.Members)
                .Include(t => t.Route)
                .Include(t => t.Season)
                .AsNoTracking()
                .Where(t => t.TourOperatorId == tourOperatorId);

            if (seasonId.HasValue)
                query = query.Where(t => t.SeasonId == seasonId.Value);

            return await query
                .OrderBy(t => t.Season.Year)
                .ThenBy(t => t.Season.Type)
                .ToListAsync(cancellationToken);
        }
    }
}
