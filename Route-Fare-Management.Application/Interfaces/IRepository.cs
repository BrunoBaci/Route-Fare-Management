using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Route_Fare_Management.Domain;

namespace Route_Fare_Management.Application.Interfaces
{
    public interface IRepository
    {
        Task<User> GetUserAsync(string email, CancellationToken token);
        Task<int> AddUserAsync(User user, CancellationToken token);
        Task<int> AddRouteAsync(Domain.Route route, CancellationToken token);
        Task<Domain.Route?> GetRouteAsync(Guid id, CancellationToken token);
        Task<List<Domain.Route>> GetRoutesAsync(string? term, CancellationToken token);

        Task<Domain.Route?> UpdateRouteAsync(Guid id,
            string origin,
            string destination,
            string description,
            List<BookingClass> bookingClasses,
            CancellationToken token);
    }
}
