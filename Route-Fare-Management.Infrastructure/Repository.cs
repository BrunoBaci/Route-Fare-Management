using MediatR;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Route_Fare_Management.Application.Interfaces;
using Route_Fare_Management.Domain;
using Route_Fare_Management.Domain.Exceptions;
using System.Linq;

namespace Route_Fare_Management.Infrastructure
{
    public class Repository : IRepository
    {   
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
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

        public async Task<int> AddUserAsync(User user, CancellationToken token)
        {
            _context.Users.Add(user);
           return await _context.SaveChangesAsync(token);
        }

        public async Task<int> AddRouteAsync(Route route, CancellationToken token)
        {
            _context.Routes.Add(route);
            return await _context.SaveChangesAsync(token);
        }

        public async Task<Route?> GetRouteAsync(Guid id, CancellationToken token)
        {
            return await _context.Routes
            .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, token);
        }

        public async Task<List<Route>> GetRoutesAsync(string? term, CancellationToken token)
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

        public async Task<Route?> UpdateRouteAsync(Guid id, 
            string origin,
            string destination,
            string description,
            List<BookingClass> bookingClasses,
            CancellationToken token)
        {
            var route = await _context.Routes
                .FindAsync(new object[] { id }, token);

            route.Update(
                origin,
                destination,
                description,
                bookingClasses);
            await _context.SaveChangesAsync(token);
            return route;
        }
    }
}
