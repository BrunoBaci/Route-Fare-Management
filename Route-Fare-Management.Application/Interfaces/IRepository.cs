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
        Task<int> AddAndSaveAsync(User user, CancellationToken token);
        Task<int> AddAndSaveAsync(Domain.Route route, CancellationToken token);
        Task<Domain.Route?> GetRouteAsync(Guid id, CancellationToken token);
        Task<List<Domain.Route>> GetRoutesAsync(string? term, CancellationToken token);

        Task<Domain.Route?> UpdateRouteAsync(Guid id,
            string origin,
            string destination,
            string description,
            List<BookingClass> bookingClasses,
            CancellationToken token);
        Task<TourOperatorRoute?> GetForPricingInitializationAsync(Guid tourOperatorId,
            Guid routeId,
            Guid seasonId,
            CancellationToken cancellationToken);

        Task AddPricingEntriesAsync(IEnumerable<PricingEntry> entries, CancellationToken ct);
        Task<TourOperatorRoute?> GetWithPricingAsync(
        Guid tourOperatorId,
        Guid routeId,
        Guid seasonId,
        CancellationToken cancellationToken);

        Task AddAsync(PricingEntry route, CancellationToken token);
        Task SaveChangesAsync(CancellationToken token);
        Task<int> AddAndSaveAsync(Domain.Season season, CancellationToken token);
        Task<Domain.Season?> GetSeasonAsync(Guid Id, CancellationToken token);
        Task<List<Domain.Season>> GetSeasonsAsync(int? year, CancellationToken cancellationToken);
        Task<bool> ExistsTourOperatorRouteAsync(Guid tourOperatorId, Guid routeId, Guid seasonId, CancellationToken ct);
        Task<Domain.TourOperator?> GetTourOperatorAsync(Guid id, CancellationToken ct);
        Task<int> AddAndSaveAsync(TourOperatorRoute tourOperatorRoute, CancellationToken token);
        Task AddAsync(TourOperatorRoute entity, CancellationToken token);
        Task AddAsync(Domain.TourOperator entity, CancellationToken ct);
        Task<Domain.TourOperator?> GetTourOperatorWithMembersAsync(Guid id, CancellationToken ct);
        Task<List<TourOperatorRoute>> GetOperatorRoutesAsync(
            Guid tourOperatorId,
            Guid? seasonId,
            CancellationToken cancellationToken);
        Task<List<Domain.TourOperator>> GetTourOperatorsAsync(CancellationToken ct, bool activeOnly = false);


    }
}